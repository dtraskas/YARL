Yet Another Rule Language (YARL)
================================

Welcome to the YARL source code! 

From this repository you can build the YARL compiler. The YARL API allows you to create rules that use fuzzy logic and can be embedded into any system.

Prerequisites
--------------
- Latest version of Visual Studio 2013
- .NET 4.5.1
- ANTLR 3


Example Code
------------
```
/*
	YARL Example Code
	version v1.0

	Dimitrios Traskas
*/

fuzzyvar water	
	range (0, 100)		
begin
	set_\ (cold, 0, 40)
	set_/\(tepid, 30, 50, 70)	
	set_/\ (hot, 50, 80, 100)
end

fuzzyvar power
	range (0, 75)	
begin
	set_/\ (low, 0, 25, 50)
	set_/\ (high, 25, 50, 75)
end

rule r1
when 
	($water is cold) or ($water is tepid)
then 
	$power is high
end

rule r2	
when 
	($water is hot)
then 
	$power is low
end

ruleset rset1 
	desc ("ruleset one")
	contains all

execute (rset1)
```

How to contribute
-----------------

One way to contribute changes is to send a GitHub [Pull Request](https://help.github.com/articles/using-pull-requests).

**To get started using GitHub:**

- Create your own YARL **fork** by clicking the __Fork button__ in the top right of this page.
- [Install a Git client](http://help.github.com/articles/set-up-git) on your computer.
- Use the GitHub program to **Sync** the project's files to a folder on your computer.
- Open up the project in Visual Studio.
- Modify the source codes and test your changes.
- Using the GitHub program, you can easily **submit contributions** back up to your **fork**.  These files will be visible to all subscribers.
- When you're ready to send the changes for review, simply create a [Pull Request](https://help.github.com/articles/using-pull-requests).

Common issues
-------------

None at the moment
