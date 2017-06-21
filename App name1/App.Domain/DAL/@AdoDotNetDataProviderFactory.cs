namespace AppData
{
    using System;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public class AdoDotNetDataProviderFactory : IDataProviderFactory
    {
        string ConnectionString, ConnectionStringKey;

        public AdoDotNetDataProviderFactory(DataProviderFactoryInfo factoryInfo)
        {
            ConnectionString = factoryInfo.ConnectionString;
            ConnectionStringKey = factoryInfo.ConnectionStringKey;
        }

        public virtual IDataProvider GetProvider(Type type)
        {
            IDataProvider result = null;

            if (type == typeof(Domain.Contact)) result = new ContactDataProvider();
            else if (type == typeof(Domain.ContactContactType)) result = new ContactContactTypeDataProvider();
            else if (type == typeof(Domain.ContactType)) result = new ContactTypeDataProvider();
            else if (type == typeof(Domain.Settings)) result = new SettingsDataProvider();
            else if (type == typeof(Domain.User)) result = new UserDataProvider();
            else if (result == null)
            {
                throw new NotSupportedException(type + " is not a data-supported type.");
            }

            return result;
        }

        public virtual bool SupportsPolymorphism() => true;
    }
}