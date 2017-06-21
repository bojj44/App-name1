namespace AppData
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public class SettingsDataProvider : SQLiteDataProvider
    {
        #region SQL Commands

        static string SELECT_COMMAND { get { return $"SELECT {COLUMNS_LIST} FROM [Settings] AS S WHERE S.[ID] = @Id"; } }

        const string COLUMNS_LIST = @"S.[ID] AS [Settings_Id],
        S.[Name] AS [Settings_Name],
        S.[MySetting1] AS [Settings_MySetting1]";

        const string INSERT_COMMAND = @"INSERT INTO [Settings]
        ([Id], [Name], [MySetting1])
        VALUES
        (@Id, @Name, @MySetting1)";

        const string UPDATE_COMMAND = @"UPDATE [Settings] SET
        [Id] = @Id,
        [Name] = @Name,
        [MySetting1] = @MySetting1
        WHERE [ID] = @OriginalId";

        const string DELETE_COMMAND = @"DELETE FROM [Settings] WHERE [ID] = @Id";

        #endregion

        #region Property-Column Mappings

        internal static Dictionary<string, string> PropertyColumnMappings = new Dictionary<string, string> {
        { "ID", "S.[ID]" },
        { "Name", "S.[Name]" },
        { "MySetting1", "S.[MySetting1]" }};

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
                    throw new Exception($"There is no Settings record with the ID of '{objectID}'.");
                }
            }
        }

        public override IEnumerable<IEntity> GetList(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.Settings>(criteria, options, PropertyColumnMappings);

            using (var reader = ExecuteReader(queryBuilder.GenerateQuery(COLUMNS_LIST, "[Settings] AS S"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters)))
            {
                var result = new List<Domain.Settings>();
                while (reader.Read())
                {
                    result.Add(Parse(reader));
                }

                return result;
            }
        }

        public override int Count(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.Settings>(criteria, options, PropertyColumnMappings);

            return (int)ExecuteScalar(queryBuilder.GenerateCountQuery("[Settings] AS S"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters));
        }

        public override IEnumerable<string> ReadManyToManyRelation(IEntity instance, string property)
        {
            throw new ArgumentException($"The property '{property}' is not supported for the instance of '{instance.GetType()}'");
        }

        internal static Domain.Settings Parse(IDataReader reader)
        {
            var result = new Domain.Settings();
            FillData(reader, result);
            EntityManager.SetSaved(result, reader[0].ToString().To<Guid>());
            return result;
        }

        internal static void FillData(IDataReader reader, Domain.Settings entity)
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            entity.Name = (string)values[1];

            entity.MySetting1 =  values[2].ToStringOrEmpty().To<int>();
        }

        public override void Save(IEntity record)
        {
            var item = record as Domain.Settings;

            if (record.IsNew)
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
        }

        void Insert(Domain.Settings item)
        {
            ExecuteScalar(INSERT_COMMAND, CommandType.Text, CreateParameters(item));
        }

        public override void BulkInsert(IEntity[] entities, int batchSize)
        {
            var commands = new List<KeyValuePair<string, IDataParameter[]>>();

            foreach (var item in entities.Cast<Domain.Settings>())
            {
                commands.Add(INSERT_COMMAND, CreateParameters(item));
            }

            ExecuteNonQuery(CommandType.Text, commands);
        }

        void Update(Domain.Settings item)
        {
            ExecuteScalar(UPDATE_COMMAND, CommandType.Text, CreateParameters(item));
        }

        IDataParameter[] CreateParameters(Domain.Settings item)
        {
            var result = new List<IDataParameter>();

            result.Add(CreateParameter("OriginalId", item.OriginalId));
            result.Add(CreateParameter("Id", item.GetId()));
            result.Add(CreateParameter("Name", item.Name));
            result.Add(CreateParameter("MySetting1", item.MySetting1));

            return result.ToArray();
        }

        public override void Delete(IEntity record)
        {
            ExecuteNonQuery(DELETE_COMMAND, System.Data.CommandType.Text, CreateParameter("Id", record.GetId()));
        }
    }
}