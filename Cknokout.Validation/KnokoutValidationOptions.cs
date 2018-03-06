namespace Knockout.Validation
{
    /// <summary>
    /// Represents ko validation options(see Knockout.validate framework)
    /// </summary>
    public class KnockoutValidationOptions
    {
        #region Properties

        public bool RegisterExtenders { get; set; }
        public bool MessagesOnModified { get; set; }
        public bool InsertMessages { get; set; }
        public bool DecorateInputElement { get; set; }
        public bool DecarateInput { get; set; }
        public string ErrorElementClass { get; set; }
        #endregion
    }
}