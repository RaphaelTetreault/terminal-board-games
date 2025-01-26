echo on

:: Delete old builds folder and contents
:: /s	Deletes a directory tree (the specified directory and all its subdirectories, including all files).
:: /q	Specifies quiet mode. Does not prompt for confirmation when deleting a directory tree.
::      The /q parameter works only if /s is also specified.
rmdir /s /q builds
