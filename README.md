# StackOverflowLoaderAPI
Rozwiązanie zadania. Wszystkie kontenery uruchomią się za pomocą komendy docker compose up.
Przy pierwszym uruchomieniu aplikacja stworzy bazę danych w kontenerze MSSQL oraz spróbuje pobrać minimalną wymaganą ilość tagów. 

W pliku docker-compose.yaml są dostępne zmienne:
- connstring= connection string do połączenia z kontenerem SQL
- tagstoload=1000 -> ilość tagów do pobrania, domyślnie 1000
- tagstoloadpagesize=100 -> wielkość strony z api SO, domyślnie 100
- timemsbeforenextcall=5000 -> oczekiwanie między zapytaniami, domyślnie 5 sekund
- resetdbonstartup=true -> resetowanie bazy danych przy każdym uruchomieniu, domyślnie prawda

Zapytanie GET do API zwróci wszystkie dostępne tagi, stronicowane i posortowane.
Domyślne sortowanie - wg. udziału rosnąco

GET Przyjmuje parametry:
- PageNumber - nr strony, domyślnie 1	
- PageSize - wielkość strony, domyślnie 10
- OrderDirection - "ascending" lub "descending"
- OrderBy -"name" lub "count"

Definicje openApi można obejrzeć domyślnie pod adresem: http://localhost:7142/swagger/index.html

Testy jednostkowe oraz integracyjne uruchomią się razem z API i bazą danych.
Testy jednostkowe mają oskryptowane opóźnienie - 60 sekund, do zmiany w pliku compose pomiędzy -t a --

"-t",
"60",
"--",

Testy integracyjne mają oskryptowane opóźnienie 80 sekund.

Testy wykonają się, zwrócą informacje czy testy zostały wykonane poprawnie i zatrzymają kontener

PUT zresetuje bazę i pobierze tagi ponownie.

