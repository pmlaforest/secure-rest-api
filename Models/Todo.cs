namespace TodoApi.Models
{  
     public class Todo 
    {
        public long Id { get; set; } 
         public string content { get; set; }
         public bool  done { get; set; }
         public long ownerId { get; set; }
    }
}