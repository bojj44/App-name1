namespace AppData
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public class UserDataProvider : SQLiteDataProvider
    {
        #region SQL Commands

        static string SELECT_COMMAND { get { return $"SELECT {COLUMNS_LIST} FROM [Users] AS U WHERE U.[ID] = @Id"; } }

        const string COLUMNS_LIST = @"U.[ID] AS [Users_Id],
        U.[FirstName] AS [Users_FirstName],
        U.[LastName] AS [Users_LastName],
        U.[Email] AS [Users_Email],
        U.[Password] AS [Users_Password],
        U.[IsDeactivated] AS [Users_IsDeactivated]";

        const string INSERT_COMMAND = @"INSERT INTO [Users]
        ([Id], [FirstName], [LastName], [Email], [Password], [IsDeactivated])
        VALUES
        (@Id, @FirstName, @LastName, @Email, @Password, @IsDeactivated)";

        const string UPDATE_COMMAND = @"UPDATE [Users] SET
        [Id] = @Id,
        [FirstName] = @FirstName,
        [LastName] = @LastName,
        [Email] = @Email,
        [Password] = @Password,
        [IsDeactivated] = @IsDeactivated
        WHERE [ID] = @OriginalId";

        const string DELETE_COMMAND = @"DELETE FROM [Users] WHERE [ID] = @Id";

        #endregion

        #region Property-Column Mappings

        internal static Dictionary<string, string> PropertyColumnMappings = new Dictionary<string, string> {
        { "ID", "U.[ID]" },
        { "FirstName", "U.[FirstName]" },
        { "LastName", "U.[LastName]" },
        { "Email", "U.[Email]" },
        { "Password", "U.[Password]" },
        { "IsDeactivated", "U.[IsDeactivated]" }};

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
                    throw new Exception($"There is no User record with the ID of '{objectID}'.");
                }
            }
        }

        public override IEnumerable<IEntity> GetList(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.User>(criteria, options, PropertyColumnMappings);

            using (var reader = ExecuteReader(queryBuilder.GenerateQuery(COLUMNS_LIST, "[Users] AS U"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters)))
            {
                var result = new List<Domain.User>();
                while (reader.Read())
                {
                    result.Add(Parse(reader));
                }

                return result;
            }
        }

        public override int Count(Type type, IEnumerable<ICriterion> criteria, params QueryOption[] options)
        {
            var queryBuilder = new SqlQueryBuilder<Domain.User>(criteria, options, PropertyColumnMappings);

            return (int)ExecuteScalar(queryBuilder.GenerateCountQuery("[Users] AS U"), System.Data.CommandType.Text, GenerateParameters(queryBuilder.Parameters));
        }

        public override IEnumerable<string> ReadManyToManyRelation(IEntity instance, string property)
        {
            throw new ArgumentException($"The property '{property}' is not supported for the instance of '{instance.GetType()}'");
        }

        internal static Domain.User Parse(IDataReader reader)
        {
            var result = new Domain.User();
            FillData(reader, result);
            EntityManager.SetSaved(result, reader[0].ToString().To<Guid>());
            return result;
        }

        internal static void FillData(IDataReader reader, Domain.User entity)
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            entity.FirstName = (string)values[1];

            entity.LastName = (string)values[2];

            entity.Email = (string)values[3];

            if (values[4] != DBNull.Value) entity.Password = (string)values[4];

            entity.IsDeactivated = Convert.ToBoolean(values[5]);
        }

        public override void Save(IEntity record)
        {
            var item = record as Domain.User;

            if (record.IsNew)
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
        }

        void Insert(Domain.User item)
        {
            ExecuteScalar(INSERT_COMMAND, CommandType.Text, CreateParameters(item));
        }

        public override void BulkInsert(IEntity[] entities, int batchSize)
        {
            var commands = new List<KeyValuePair<string, IDataParameter[]>>();

            foreach (var item in entities.Cast<Domain.User>())
            {
                commands.Add(INSERT_COMMAND, CreateParameters(item));
            }

            ExecuteNonQuery(CommandType.Text, commands);
        }

        void Update(Domain.User item)
        {
            ExecuteScalar(UPDATE_COMMAND, CommandType.Text, CreateParameters(item));
        }

        IDataParameter[] CreateParameters(Domain.User item)
        {
            var result = new List<IDataParameter>();

            result.Add(CreateParameter("OriginalId", item.OriginalId));
            result.Add(CreateParameter("Id", item.GetId()));
            result.Add(CreateParameter("FirstName", item.FirstName));
            result.Add(CreateParameter("LastName", item.LastName));
            result.Add(CreateParameter("Email", item.Email));
            result.Add(CreateParameter("Password", item.Password));
            result.Add(CreateParameter("IsDeactivated", item.IsDeactivated));

            return result.ToArray();
        }

        public override void Delete(IEntity record)
        {
            ExecuteNonQuery(DELETE_COMMAND, System.Data.CommandType.Text, CreateParameter("Id", record.GetId()));
        }
    }
}