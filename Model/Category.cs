﻿namespace ReturnToStonks
{
  public class Category
  {
    public Category(string name, string? symbol = null)
    {
      Name = name;
      Symbol = symbol;
    }

    public string Name { get; set; }
    public string? Symbol { get; set; }
  }
}
