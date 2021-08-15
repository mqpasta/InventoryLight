#!/bin/bash

sleep 20s

 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "King.1234" -d master -i setup.sql
 sleep 1s
 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "King.1234" -d master -i InsertProducts.sql

