using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Reflection;

namespace Volo.Abp.Data
{
    public class CommonDbContextRegistrationOptions : ICommonDbContextRegistrationOptionsBuilder
    {
        //TODO: Provide an option to set base repository classes, instead of defaults.

        public bool RegisterDefaultRepositories { get; private set; }

        public bool IncludeAllEntitiesForDefaultRepositories { get; private set; }

        public Dictionary<Type, Type> CustomRepositories { get; }

        public CommonDbContextRegistrationOptions()
        {
            CustomRepositories = new Dictionary<Type, Type>();
        }

        public ICommonDbContextRegistrationOptionsBuilder WithDefaultRepositories(bool includeAllEntities = false)
        {
            RegisterDefaultRepositories = true;
            IncludeAllEntitiesForDefaultRepositories = includeAllEntities;
            return this;
        }

        public ICommonDbContextRegistrationOptionsBuilder WithCustomRepository<TEntity, TRepository>()
        {
            WithCustomRepository(typeof(TEntity), typeof(TRepository));
            return this;
        }

        private void WithCustomRepository(Type entityType, Type repositoryType)
        {
            if (!ReflectionHelper.IsAssignableToGenericType(entityType, typeof(IEntity<>)))
            {
                throw new AbpException($"Given entityType is not an entity: {entityType.AssemblyQualifiedName}. It must implement {typeof(IEntity<>).AssemblyQualifiedName}.");
            }

            if (!ReflectionHelper.IsAssignableToGenericType(repositoryType, typeof(IRepository<,>)))
            {
                throw new AbpException($"Given repositoryType is not a repository: {entityType.AssemblyQualifiedName}. It must implement {typeof(IRepository<,>).AssemblyQualifiedName}.");
            }

            CustomRepositories[entityType] = repositoryType;
        }

        public bool ShouldRegisterDefaultRepositoryFor(Type entityType)
        {
            if (!RegisterDefaultRepositories)
            {
                return false;
            }

            if (CustomRepositories.ContainsKey(entityType))
            {
                return false;
            }

            if (!IncludeAllEntitiesForDefaultRepositories && !ReflectionHelper.IsAssignableToGenericType(entityType, typeof(IAggregateRoot<>)))
            {
                return false;
            }

            return true;
        }
    }
}