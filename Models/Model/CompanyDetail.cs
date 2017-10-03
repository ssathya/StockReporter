using System.ComponentModel.DataAnnotations;

namespace Models.Model
{
    public class CompanyDetail
    {
        #region Public Properties

        public int Id { get; set; }

        public string Industry { get; set; }

        public int IPOyear { get; set; }

        public bool IsExTrdFund { get; set; }

        public bool IsMutualFund { get; set; }

        public string Sector { get; set; }

        [Required]
        public string SecurityName { get; set; }

        [Required]
        public string Symbol { get; set; }

        #endregion Public Properties
    }
}