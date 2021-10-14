Growing a Gilded Rose
===

This is (yet another) take on the [Gilded Rose Kata][8], originally conceived by [Terry Hughes][7]. 

### Overview

The goal of the original kata was to work through the challenges of extending -- and more importantly understanding -- a
piece of legacy software. In particular, software lacking tests and written in a peculiar manner and with very
definitive constraints (both functional and political). In that same spirit, this attempt solves the kata while
_incrementally_ introducing several new concepts. This repos is meant to be a companion to a [series of blog post][0] 
about this topic. Please refer to them for in-depth explanations.

---

#### Pre-requisites

To run this code (or modify it) locally, you will need:

+ A version of the .NET SDK capable of supporting, at least, C# 9.0 _and_ F# 5.0 (for details, see [official docs][9]).
+ Optionally, a text-editor or IDE which is capability of editing and compiling C# and/or F#.

---

#### Repository structure

To help make things a bit more manageable, each branch in this repo represents a specific step in the overall journey.

 Branch       | Blog Post                  | Summary
--------------|----------------------------|----------------------------------------------------------------------------
 `0_original` | [Growing a Gilded Rose][0] | Original (in C#) console application, i.e. the start of the kata.
 `1_testable` | [Gilded Rose: Step 1][1]   | Introduces (in F#): approval tests, unit tests, property-based tests.
 `2_model-fs` | [Gilded Rose: Step 2][2]   | Introduces (in F#): functional requirements expressed as a domain model.
 `3_coalesce` | [Gilded Rose: Step 3][3]   | Demonstrates adding (new) F# code to a (legacy) C# codebase.
 `4_extended` | [Gilded Rose: Step 4][4]   | Extends previous work with new functional requirements.
 `5_fs-alone` | [Gilded Rose: Bonus 1][5]  | BONUS: replaces C# console application with F# equivalent.
 `6_model-cs` | [Gilded Rose: Bonus 2][6]  | BONUS: replaces F# domain model with C# equivalent (retains F# test suite). 

The first time you work with the repository (or if after performing `git clean -xfd`), you'll likely need to run: 

` > dotnet tool restore && dotnet paket restore`.

Subsequent to that, you can use the editor or IDE of your choice. But the following CLI commands might be useful (note,
they all assume you are in the root folder of the repository... where the `GrowingGildedRose.sln` file lives):

+ Build all the projects: `> dotnet build`.
+ Run the whole test suite: `> dotnet test`.
+ Run just the main executable: `> dotnet run -p source/GildedRose`

---

#### Problem Statement

Please note, this is taken directly from the [original kata][8].

> Hi and welcome to team Gilded Rose. As you know, we are a small inn with a prime location in a prominent city ran by a
> friendly innkeeper named Allison. We also buy and sell only the finest goods. Unfortunately, our goods are constantly
> degrading in quality as they approach their sell by date. We have a system in place that updates our inventory for us.
> It was developed by a no-nonsense type named Leeroy, who has moved on to new adventures. Your task is to add the new
> feature to our system so that we can begin selling a new category of items. First an introduction to our system:
> 
> - All items have a SellIn value which denotes the number of days we have to sell the item
> - All items have a Quality value which denotes how valuable the item is
> - At the end of each day our system lowers both values for every item
> 
> Pretty simple, right? Well this is where it gets interesting:
> 
> - Once the sell by date has passed, Quality degrades twice as fast
> - The Quality of an item is never negative
> - "Aged Brie" actually increases in Quality the older it gets
> - The Quality of an item is never more than 50
> - "Sulfuras", being a legendary item, never has to be sold or decreases in Quality
> - "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; Quality increases by 2 
>   when there are 10 days or less and by 3 when there are 5 days or less but Quality drops to 0 after the concert
> 
> We have recently signed a supplier of conjured items. This requires an update to our system:
> 
> - "Conjured" items degrade in Quality twice as fast as normal items
> 
> Feel free to make any changes to the UpdateQuality method and add any new code as long as everything still works
> correctly. However, do not alter the Item class or Items property as those belong to the goblin in the corner who will
> insta-rage and one-shot you as he doesn't believe in shared code ownership (you can make the UpdateQuality method and
> Items property static if you like, we'll cover for you).
> 
> Just for clarification, an item can never have its Quality increase above 50, however "Sulfuras" is a legendary item 
> and as such its Quality is 80 and it never alters.

[0]: https://paul.blasuc.ci/posts/grow-a-rose.html
[1]: https://paul.blasuc.ci/posts/rose-1-testable.html
[2]: https://paul.blasuc.ci/posts/rose-2-model-fs.html
[3]: https://paul.blasuc.ci/posts/rose-3-coalesce.html
[4]: https://paul.blasuc.ci/posts/rose-4-extended.html
[5]: https://paul.blasuc.ci/posts/rose-5-fs-alone.html
[6]: https://paul.blasuc.ci/posts/rose-6-model-cs.html
[7]: https://twitter.com/TerryHughes
[8]: https://github.com/NotMyself/GildedRose
[9]: https://dotnet.microsoft.com/download
