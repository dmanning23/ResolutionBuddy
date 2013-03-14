RandomExtensions
================

c# random extensions for range of doubles... also random name generator, just for fun :)

Just add reference to your project, add the "using RandomExtensions;" at the top of your file, and start using the just like the rest of the functions that are built into the Random class:

```
Random rnd = new Random();
double myDouble = rnd.NextDouble(0.0, 100.0);
float smallFloat = rnd.NextFloat();
float myFloat = rnd.NextFloat(0.0, 100.0);

StringBuilder curse = new StringBuilder("For all I care, ");
curse.Append(rnd.NextName(2, 10));
curse.Append(" can go ");
curse.Append(rnd.NextWord(4, 4).ToUpper());
curse.Append(" his own ");
curse.Append(rnd.NextWord(4, 4).ToUpper());
curse.Append("!");

//whatever
```