namespace AppData
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public class ContactContactTypeDataProvider : SQLiteDataProvider
    {
        #region SQL Commands

        static string SELECT_COMMAND { get { return $"SELECT {COLUMNS_LIST} FROM [ContactContactTypes] AS C WHERE C.[ID] = @Id"; } }

        const string COLUMNS_LIST = @"C.[ID] AS [ContactContactTypes_Id],
        C.[Contact] AS [ContactContactTypes_Contact],
        C.[ContactType] AS [ContactContactTypes_ContactType]";

        const string INSERT_COMMAND = @"INSERT INTO [ContactContactTypes]
        ([Id], [Contact], [ContactType])
        VALUES
        (@Id, @Contact, @ContactType)";

        const string UPDATE_COMMAND = @"UPDATE [ContactContactTypes] SET
        [Id] = @Id,
        [Contact] = @Contact,
        [ContactType] = @ContactType
        WHERE [ID] = @OriginalId";

        const string DELETE_COMMAND = @"DELETE FROM [ContactContactTypes] WHERE [ID] = @Id";

        #endregion

        #region Property-Column Mappings

        internal static Dictionary<string, string> PropertyColumnMappings = new Dictionary<string, string> {
        { "ID", "C.[ID]" },
        { "Contact", "C.[Contact]" },
        { "ContactType", "C.[ContactType]" }};

        #endregion

        #region Subquery Mappings

        static IEnumerable<PropertySubqueryMapping> GetSubQueryMappings()
        {
            yield return new PropertySubqueryMapping("Contact.*", "ContactContactTypes_Contact_", ContactDataProvider.PropertyColumnMappings)
            {
                Subquery = "SELECT [ContactContactTypes_Contact_C].[ID] FROM [Contacts] AS ContactContactTypes_Contact_C WHERE [ContactContactTypes_Contact_C].[ID] = C.[Contact]"
            };

            yield return new PropertySubqueryMapping("ContactType.*", "ContactContactTypes_ContactType_", ContactTypeDataProvider.PropertyColumnMappings)
            {
                Subquery = "SELECT [ContactContactTypes_ContactType_C].[ID] FROM [ContactTypes] AS ContactContactTypes_ContactType_C WHERE [ContactContactTypes_ContactType_C].[ID] = C.[ContactType]"
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
                    throw new Exception($"There is no Contact contact type record with the ID of '{objectID}'.");
                }
            }
        }

        public override IEnumerable<IEntity> GetList(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.ContactContactType>(criteria, options, PropertyColumnMappings, GetSubQueryMappings);

            using (var reader = ExecuteReader(queryBuilder.GenerateQuery(COLUMNS_LIST, "[ContactContactTypes] AS C"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters)))
            {
                var result = new List<Domain.ContactContactType>();
                while (reader.Read())
                {
                    result.Add(Parse(reader));
                }

                return result;
            }
        }

        public override int Count(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.ContactContactType>(criteria, options, PropertyColumnMappings, GetSubQueryMappings);

            return (int)ExecuteScalar(queryBuilder.GenerateCountQuery("[ContactContactTypes] AS C"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters));
        }

        public override IEnumerable<string> ReadManyToManyRelation(IEntity instance, string property)
        {
            throw new ArgumentException($"The property '{property}' is not supported for the instance of '{instance.GetType()}'");
        }

        internal static Domain.ContactContactType Parse(IDataReader reader)
        {
            var result = new Domain.ContactContactType();
            FillData(reader, result);
            EntityManager.SetSaved(result, reader[0].ToString().To<Guid>());
            return result;
        }

        internal static void FillData(IDataReader reader, Domain.ContactContactType entity)
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            if (values[1] != DBNull.Value) entity.ContactId =  values[1].ToStringOrEmpty().To<Guid>();

            if (values[2] != DBNull.Value) entity.ContactTypeId =  values[2].ToStringOrEmpty().To<Guid>();
        }

        public override void Save(IEntity record)
        {
            var item = record as Domain.ContactContactType;

            if (record.IsNew)
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
        }

        void Insert(Domain.ContactContactType item)
        {
            ExecuteScalar(INSERT_COMMAND, CommandType.Text, CreateParameters(item));
        }

        public override void BulkInsert(IEntity[] entities, int batchSize)
        {
            var commands = new List<KeyValuePair<string, IDataParameter[]>>();

            foreach (var item in entities.Cast<Domain.ContactContactType>())
            {
                commands.Add(INSERT_COMMAND, CreateParameters(item));
            }

            ExecuteNonQuery(CommandType.Text, commands);
        }

        void Update(Domain.ContactContactType item)
        {
            ExecuteScalar(UPDATE_COMMAND, CommandType.Text, CreateParameters(item));
        }

        IDataParameter[] CreateParameters(Domain.ContactContactType item)
        {
            var result = new List<IDataParameter>();

            result.Add(CreateParameter("OriginalId", item.OriginalId));
            result.Add(CreateParameter("Id", item.GetId()));
            result.Add(CreateParameter("Contact", item.ContactId));
            result.Add(CreateParameter("ContactType", item.ContactTypeId));

            return result.ToArray();
        }

        public override void Delete(IEntity record)
        {
            ExecuteNonQuery(DELETE_COMMAND, System.Data.CommandType.Text, CreateParameter("Id", record.GetId()));
        }
    }
}