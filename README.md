# Run Signature Analysis
Compile the main .cs file with CSC and run from the command line.

# How To Use
This program takes 3 args:
1. Source directory to scan
2. Path to output the CSV file, including the desired name
3. *Any* text to indicate that all sub-directories should be scanned (ommission of this arg indicates only the source directory should be scanned)

Example: SignatureAnalysis.exe "C:\Program Files\" "C:\Users\Daxston\Desktop\Output.csv" arg3FlagThisCanBeAnySpacelessText
