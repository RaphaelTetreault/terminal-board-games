::echo on

:: Delete old builds folder and contents
:: /s	Deletes a directory tree (the specified directory and all its subdirectories, including all files).
:: /q	Specifies quiet mode. Does not prompt for confirmation when deleting a directory tree.
::      The /q parameter works only if /s is also specified.
::rmdir /s /q builds
::mkdir builds

::set "runtime=win-x64"
set runtime=%1

:: Move to project folder and build
dotnet publish battleship             -c Release -r %runtime% -o builds/battleship-%runtime%
dotnet publish battleship-simple      -c Release -r %runtime% -o builds/battleship-simple-%runtime%
dotnet publish mastermind             -c Release -r %runtime% -o builds/mastermind-%runtime%
dotnet publish mastermind-simple      -c Release -r %runtime% -o builds/mastermind-simple-%runtime%
dotnet publish melting-snowman        -c Release -r %runtime% -o builds/melting-snowman-%runtime%
dotnet publish melting-snowman-simple -c Release -r %runtime% -o builds/melting-snowman-simple-%runtime%
dotnet publish wordle                 -c Release -r %runtime% -o builds/wordle-%runtime%

:: Wordle does not build with words list
robocopy wordle/valid-wordle-words.txt builds/wordle-%runtime%

:: Package builds as ZIP

tar.exe -a -c -f builds/battleship-%runtime%.zip                builds/battleship-%runtime%
tar.exe -a -c -f builds/battleship-simple-%runtime%.zip         builds/battleship-simple-%runtime%
tar.exe -a -c -f builds/mastermind-%runtime%.zip                builds/mastermind-%runtime%
tar.exe -a -c -f builds/mastermind-simple-%runtime%.zip         builds/mastermind-simple-%runtime%
tar.exe -a -c -f builds/melting-snowman-%runtime%.zip           builds/melting-snowman-%runtime%
tar.exe -a -c -f builds/melting-snowman-simple-%runtime%.zip    builds/melting-snowman-simple-%runtime%
tar.exe -a -c -f builds/wordle-%runtime%.zip                    builds/wordle-%runtime%