@echo off
cls
echo ***************************************************************************
echo *** Cave Unity Config Tool - Sample Script                              ***
echo ***************************************************************************
echo.
echo In the directory 'user_scripts' you can define your own scripts to extend 
echo functionality of this tool.
echo Every script will be start with two arguments inclusive quotation marks:
echo    1. full path of project directory
echo       * %1
echo    2. name of project
echo       * %2
echo If the directory 'user_scripts' doesn't exist or it is empty, you cannot
echo see the menu option 'User's Scripts'.
echo.
pause