RandomExtensions
================

c# random extensions for range of doubles... omg I use this EVERYWHERE it's so useful

Also random name and swear word generator, just for fun :)

Just add reference to your project, add the "using RandomExtensions;" at the top of your file, and start using the just like the rest of the functions that are built into the Random class:

```
Random rnd = new Random();
double myDouble = rnd.NextDouble(0.0, 100.0);
float smallFloat = rnd.NextFloat();
float myFloat = rnd.NextFloat(0.0, 100.0);

StringBuilder curse = new StringBuilder("For all I care, ");
curse.Append(rnd.NextName(2, 10));
curse.Append(" can go ");
curse.Append(rnd.NextSwearWord().ToUpper());
curse.Append(" his own ");
curse.Append(rnd.NextSwearWord().ToUpper());
curse.Append("!!!");

//whatever
```