namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Input used to query dynamic entity property values.
    /// </summary>
    public class GetDynamicEntityPropertyValuesInput
    {
        /// <summary>
        /// Full CLR type name of the entity.
        /// </summary>
        public string EntityFullName { get; set; }

        /// <summary>
        /// Identifier of the entity instance.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Identifier of the dynamic entity property binding.
        /// </summary>
        public int DynamicEntityPropertyId { get; set; }

        /// <summary>
        /// Identifier of the dynamic property.
        /// </summary>
        public int DynamicPropertyId { get; set; }

        /// <summary>
        /// Name of the dynamic property.
        /// </summary>
        public string PropertyName { get; set; }
    }
}
