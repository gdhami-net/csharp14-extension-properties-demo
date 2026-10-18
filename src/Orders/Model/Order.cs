namespace Shop.Model;

public class Order
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public DateTime DueDate { get; set; }
    public DateTime? PaidOn { get; set; }
}
