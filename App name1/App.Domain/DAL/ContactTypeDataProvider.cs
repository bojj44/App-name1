namespace AppData
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public class ContactTypeDataProvider : SQLiteDataProvider
    {
        #region SQL Commands

        static string SELECT_COMMAND { get { return $"SELECT {COLUMNS_LIST} FROM [ContactTypes] AS C WHERE C.[ID] = @Id"; } }

        const string COLUMNS_LIST = @"C.[ID] AS [ContactTypes_Id], C.[Name] AS [ContactTypes_Name]";

        const string INSERT_COMMAND = @"INSERT INTO [ContactTypes]
        ([Id], [Name])
        VALUES
        (@Id, @Name)";

        const string UPDATE_COMMAND = @"UPDATE [ContactTypes] SET
        [Id] = @Id,
        [Name] = @Name
        WHERE [ID] = @OriginalId";

        const string DELETE_COMMAND = @"DELETE FROM [ContactTypes] WHERE [ID] = @Id";

        #endregion

        #region Property-Column Mappings

        internal static Dictionary<string, string> PropertyColumnMappings = new Dictionary<string, string> {
        { "ID", "C.[ID]" },
        { "Name", "C.[Name]" }};

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
                    throw new Exception($"There is no Contact type record with the ID of '{objectID}'.");
                }
            }
        }

        public override IEnumerable<IEntity> GetList(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.ContactType>(criteria, options, PropertyColumnMappings);

            using (var reader = ExecuteReader(queryBuilder.GenerateQuery(COLUMNS_LIST, "[ContactTypes] AS C"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters)))
            {
                var result = new List<Domain.ContactType>();
                while (reader.Read())
                {
                    result.Add(Parse(reader));
                }

                return result;
            }
        }

        public override int Count(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.ContactType>(criteria, options, PropertyColumnMappings);

            return (int)ExecuteScalar(queryBuilder.GenerateCountQuery("[ContactTypes] AS C"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters));
        }

        public override IEnumerable<string> ReadManyToManyRelation(IEntity instance, string property)
        {
            throw new ArgumentException($"The property '{property}' is not supported for the instance of '{instance.GetType()}'");
        }

        internal static Domain.ContactType Parse(IDataReader reader)
        {
            var result = new Domain.ContactType();
            FillData(reader, result);
            EntityManager.SetSaved(result, reader[0].ToString().To<Guid>());
            return result;
        }

        internal static void FillData(IDataReader reader, Domain.ContactType entity)
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            entity.Name = (string)values[1];
        }

        public override void Save(IEntity record)
        {
            var item = record as Domain.ContactType;

            if (record.IsNew)
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
        }

        void Insert(Domain.ContactType item)
        {
            ExecuteScalar(INSERT_COMMAND, CommandType.Text, CreateParameters(item));
        }

        public override void BulkInsert(IEntity[] entities, int batchSize)
        {
            var commands = new List<KeyValuePair<string, IDataParameter[]>>();

            foreach (var item in entities.Cast<Domain.ContactType>())
            {
                commands.Add(INSERT_COMMAND, CreateParameters(item));
            }

            ExecuteNonQuery(CommandType.Text, commands);
        }

        void Update(Domain.ContactType item)
        {
            ExecuteScalar(UPDATE_COMMAND, CommandType.Text, CreateParameters(item));
        }

        IDataParameter[] CreateParameters(Domain.ContactType item)
        {
            var result = new List<IDataParameter>();

            result.Add(CreateParameter("OriginalId", item.OriginalId));
            result.Add(CreateParameter("Id", item.GetId()));
            result.Add(CreateParameter("Name", item.Name));

            return result.ToArray();
        }

        public override void Delete(IEntity record)
        {
            ExecuteNonQuery(DELETE_COMMAND, System.Data.CommandType.Text, CreateParameter("Id", record.GetId()));
        }
    }
}