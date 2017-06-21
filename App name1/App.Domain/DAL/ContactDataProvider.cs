namespace AppData
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public class ContactDataProvider : SQLiteDataProvider
    {
        #region SQL Commands

        static string SELECT_COMMAND { get { return $"SELECT {COLUMNS_LIST} FROM [Contacts] AS C WHERE C.[ID] = @Id"; } }

        const string COLUMNS_LIST = @"C.[ID] AS [Contacts_Id],
        C.[Name] AS [Contacts_Name],
        C.[Email] AS [Contacts_Email],
        C.[Tel] AS [Contacts_Tel],
        C.[Notes] AS [Contacts_Notes],
        C.[Type] AS [Contacts_Type],
        C.[DateOfBirth] AS [Contacts_DateOfBirth],
        C.[TimeOfBirth] AS [Contacts_TimeOfBirth],
        C.[Photo_FileName] AS [Contacts_Photo_FileName],
        C.[Number] AS [Contacts_Number],
        C.[IsNice] AS [Contacts_IsNice]";

        const string INSERT_COMMAND = @"INSERT INTO [Contacts]
        ([Id], [Name], [Email], [Tel], [Notes], [Type], [DateOfBirth], [TimeOfBirth], [Photo_FileName], [Number], [IsNice])
        VALUES
        (@Id, @Name, @Email, @Tel, @Notes, @Type, @DateOfBirth, @TimeOfBirth, @Photo_FileName, @Number, @IsNice)";

        const string UPDATE_COMMAND = @"UPDATE [Contacts] SET
        [Id] = @Id,
        [Name] = @Name,
        [Email] = @Email,
        [Tel] = @Tel,
        [Notes] = @Notes,
        [Type] = @Type,
        [DateOfBirth] = @DateOfBirth,
        [TimeOfBirth] = @TimeOfBirth,
        [Photo_FileName] = @Photo_FileName,
        [Number] = @Number,
        [IsNice] = @IsNice
        WHERE [ID] = @OriginalId";

        const string DELETE_COMMAND = @"DELETE FROM [Contacts] WHERE [ID] = @Id";

        #endregion

        #region Property-Column Mappings

        internal static Dictionary<string, string> PropertyColumnMappings = new Dictionary<string, string> {
        { "ID", "C.[ID]" },
        { "Name", "C.[Name]" },
        { "Email", "C.[Email]" },
        { "Tel", "C.[Tel]" },
        { "Notes", "C.[Notes]" },
        { "Type", "C.[Type]" },
        { "DateOfBirth", "C.[DateOfBirth]" },
        { "TimeOfBirth", "C.[TimeOfBirth]" },
        { "Photo", "C.[Photo_FileName]" },
        { "Number", "C.[Number]" },
        { "IsNice", "C.[IsNice]" }};

        #endregion

        #region Subquery Mappings

        static IEnumerable<PropertySubqueryMapping> GetSubQueryMappings()
        {
            yield return new PropertySubqueryMapping("Type.*", "Contacts_Type_", ContactTypeDataProvider.PropertyColumnMappings)
            {
                Subquery = "SELECT [Contacts_Type_C].[ID] FROM [ContactTypes] AS Contacts_Type_C WHERE [Contacts_Type_C].[ID] = C.[Type]"
            };
        }

        #endregion

        public override IEntity Get(Type type, object objectID)
        {
            using (var reader = ExecuteReader(SELECT_COMMAND, System.Data.CommandType.Text, CreateParameter("Id", objectID)))
            {
                if (reader.Read())
                {
                    return Parse(reader);
                }
                else
                {
                    throw new Exception($"There is no Contact record with the ID of '{objectID}'.");
                }
            }
        }

        public override IEnumerable<IEntity> GetList(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.Contact>(criteria, options, PropertyColumnMappings, GetSubQueryMappings);

            using (var reader = ExecuteReader(queryBuilder.GenerateQuery(COLUMNS_LIST, "[Contacts] AS C"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters)))
            {
                var result = new List<Domain.Contact>();
                while (reader.Read())
                {
                    result.Add(Parse(reader));
                }

                return result;
            }
        }

        public override int Count(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.Contact>(criteria, options, PropertyColumnMappings, GetSubQueryMappings);

            return (int)ExecuteScalar(queryBuilder.GenerateCountQuery("[Contacts] AS C"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters));
        }

        public override IEnumerable<string> ReadManyToManyRelation(IEntity instance, string property)
        {
            throw new ArgumentException($"The property '{property}' is not supported for the instance of '{instance.GetType()}'");
        }

        internal static Domain.Contact Parse(IDataReader reader)
        {
            var result = new Domain.Contact();
            FillData(reader, result);
            EntityManager.SetSaved(result, reader[0].ToString().To<Guid>());
            return result;
        }

        internal static void FillData(IDataReader reader, Domain.Contact entity)
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            entity.Name = (string)values[1];

            if (values[2] != DBNull.Value) entity.Email = (string)values[2];

            if (values[3] != DBNull.Value) entity.Tel = (string)values[3];

            if (values[4] != DBNull.Value) entity.Notes = (string)values[4];

            if (values[5] != DBNull.Value) entity.TypeId =  values[5].ToStringOrEmpty().To<Guid>();

            if (values[6] != DBNull.Value) entity.DateOfBirth = (values[6] as string)?.To<DateTime>().Date;

            if (values[7] != DBNull.Value) entity.TimeOfBirth = values[7].ToString().TryParseAs<TimeSpan>();

            if (values[8] != DBNull.Value) entity.Photo = new Document { FileName = values[8] as string};

            entity.Number =  values[9].ToStringOrEmpty().To<int>();

            if (values[10] != DBNull.Value) entity.IsNice = Convert.ToBoolean(values[10]);
        }

        public override void Save(IEntity record)
        {
            var item = record as Domain.Contact;

            if (record.IsNew)
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
        }

        void Insert(Domain.Contact item)
        {
            ExecuteScalar(INSERT_COMMAND, CommandType.Text, CreateParameters(item));
        }

        public override void BulkInsert(IEntity[] entities, int batchSize)
        {
            var commands = new List<KeyValuePair<string, IDataParameter[]>>();

            foreach (var item in entities.Cast<Domain.Contact>())
            {
                commands.Add(INSERT_COMMAND, CreateParameters(item));
            }

            ExecuteNonQuery(CommandType.Text, commands);
        }

        void Update(Domain.Contact item)
        {
            ExecuteScalar(UPDATE_COMMAND, CommandType.Text, CreateParameters(item));
        }

        IDataParameter[] CreateParameters(Domain.Contact item)
        {
            var result = new List<IDataParameter>();

            result.Add(CreateParameter("OriginalId", item.OriginalId));
            result.Add(CreateParameter("Id", item.GetId()));
            result.Add(CreateParameter("Name", item.Name));
            result.Add(CreateParameter("Email", item.Email));
            result.Add(CreateParameter("Tel", item.Tel));
            result.Add(CreateParameter("Notes", item.Notes));
            result.Add(CreateParameter("Type", item.TypeId));
            result.Add(CreateParameter("DateOfBirth", item.DateOfBirth, DbType.DateTime2));
            result.Add(CreateParameter("TimeOfBirth", item.TimeOfBirth));
            result.Add(CreateParameter("Photo_FileName", item.Photo));
            result.Add(CreateParameter("Number", item.Number));
            result.Add(CreateParameter("IsNice", item.IsNice));

            return result.ToArray();
        }

        public override void Delete(IEntity record)
        {
            ExecuteNonQuery(DELETE_COMMAND, System.Data.CommandType.Text, CreateParameter("Id", record.GetId()));
        }
    }
}