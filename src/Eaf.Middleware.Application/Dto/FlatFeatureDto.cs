namespace Eaf.Middleware.Editions.Dto
{
    /// <summary>
    /// Representa a classe FlatFeatureDto.
    /// </summary>
    public class FlatFeatureDto
    {
        /// <summary>
        /// Obtém ou define DefaultValue.
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// Obtém ou define Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Obtém ou define DisplayName.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Obtém ou define InputType.
        /// </summary>
        public FeatureInputTypeDto InputType { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define ParentName.
        /// </summary>
        public string ParentName { get; set; }
    }
}