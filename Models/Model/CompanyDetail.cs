using System.ComponentModel.DataAnnotations;

namespace Models.Model
{
    public class CompanyDetail
    {
        #region Public Properties

        public int Id { get; set; }

        [Required]
        public string SecurityName { get; set; }

        [Required]
        public string Symbol { get; set; }

        public bool IsExTrdFund { get; set; }
        public bool IsMutualFund { get; set; }
    }

    #endregion Public Properties
}