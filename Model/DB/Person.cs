namespace ReturnToStonks
{
    public class Person
    {
        public Person(string? first_Name, string? last_Name, string? contact_Method, string? contact_ID)
        {
            First_Name = first_Name;
            Last_Name = last_Name;
            Contact_Method = contact_Method;
            Contact_ID = contact_ID;
        }

        public Person(Person person)
        {
            First_Name = person.First_Name;
            Last_Name = person.Last_Name;
            Contact_Method = person.Contact_Method;
            Contact_ID = person.Contact_ID;
        }

        public Person()
        {
        }

        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Contact_Method { get; set; }
        public string? Contact_ID { get; set; }
    }
}
