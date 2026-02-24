namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
    public class DD11DataModel
    {
        public List<DD11ViewModel>? DD11List { get; set; }

        public DD11DataModel()
        {
            DD11List = new List<DD11ViewModel>();
        }
    }
}
