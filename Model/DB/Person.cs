namespace ReturnToStonks
{
    public class Person
    {
        public Person(string name, string? contact_Method, string? contact_ID)
        {
            Name = name;
            Contact_Method = contact_Method;
            Contact_ID = contact_ID;
        }

        public Person(Person person)
        {
            Name = person.Name;
            Contact_Method = person.Contact_Method;
            Contact_ID = person.Contact_ID;
        }

        public Person()
        {
            Name = string.Empty;
        }

        public string Name { get; set; }
        public string? Contact_Method { get; set; }
        public string? Contact_ID { get; set; }
    }
}
