# Localised Resource Extraction Benchmark

Here's a quick link to the [results](#results).

One of the systems I worked on was a statistical datawarehouse application. Each dataset has several dimensions, each dimension has several dimension members, and each dimension member can have labels in multiple languages.

I wanted to investigate the fastest way of extracting the labels. In particular, the case where labels in all languages are required. The system is years old, and schema changes are very difficult to do, so I only investigated extracting from our current schema. I did not investigate if there are schema changes that could make it more faster.

I made all of the extraction methods need to return a list of immutable, strongly typed objects, to ensure that quicker SQL queries aren't being traded off for longer processing in .NET.

## Schema

You can check the DbInitialise class in the Setup folder for the create table definitions. A quick description is that there's a single dictionary table that contains all labels for everything with translations. Tables that contain objects with labels have a dictionary key, to look up in the dictionary table. In this benchmark application, there is only a single source table, so the dictionary table and source table IDs match. However, this is not the case in our production system, hence why I don't have a foreign key relationship between the tables.

## Extraction methods

### Single Language
This is added as a reference to compare how much faster it is to extract the source data in only a single language.

### Basic Join
This performs a single join on the dictionary table, so the result set contains multiple lines per source row. The extractor needs to find matches, to add to the object's label dictionary, before returning the result.

### Basic Join As XML
The exact same query as the Basic Join, just with SQL Server's "FOR XML AUTO" added to the end. This means the .NET code didn't need to find duplicate rows, but instead does XML processing.

### Labels As Joined Columns
This generates a dynamic SQL query, based on the numer, and names, of the languages the application is configured for, by joining the labels table once per language. This means there's just one row in the result set for each row in the source table, and the SQL database needs to transfer less data to the application, since there's significantly less duplicated data.

### Labels As Pivoted Columns
This generates a dynamic SQL query, but it joins the labels table once, like Basic Join, but then pivots the result to generate a result table with one row per source row, and one column per language.

### Labels As XML
This query also returns one row in the result set for each row in the source table. It also has a static result set structure. It does this by doing a sub query (could call a scalar value function) that retuns all the labels as XML in the label column of the result set. The application then needs to parse the XML and transform it into the application's model data structure.

### Local Join
Send two queries to the database, one to get the whole source table, another the get the labels from the dictionary for the source rows that were retrieved. Convert all retrieved results into strongly typed classes for fast access and lookup, then do all necessary joins in C#. This tests if specialised data structures in a strongly typed language is faster than SQL, although the C# code is single-threaded.

## <a id="results">Results</a>

|Extraction Method|Time to complete|Memory Allocated|GC collections per 1k runs (gen0 gen1 gen2)|
|-----------------|----------------|----------------|-------------------------------------------|
|SingleLanguage|86 ms (55%)|11.8 MB|2200 1000 300|
|LabelsAsJoinedColumns|157 ms (100%)|31.8 MB|6200 2900 900|
|LocalJoin|193 ms (123%)|45.7 MB|8500 3500 1500|
|BasicJoin|222 ms (142%)|41.6 MB|8100 3400 1200|
|BasicJoinAsXml|227 ms (145%)|23.1 MB|4400 2000 600|
|LabelsAsPivotedColumns|251 ms (160%)|31.8 MB|6300 2900 1000|
|LabelsAsXml|345 ms (220%)|260.1 MB|44700 9800 1100|
