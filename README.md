# HtmlSelectorsLibrary
Generator selektorów dla obiektów HTML

Selektor to ciąg znaków pozwalający na znalezienie poszczególnych obiektów HTML w [drzewie DOM](https://pl.wikipedia.org/wiki/Obiektowy_model_dokumentu).

Przykład selektora:

`#mw-content-text > p:nth-child(3) > b:nth-child(1)`

Biblioteka pozwala także na łączenie ze sobą selektorów (znajdowanie wspólnych schematów).

###Przykład połączonych selektorów:

`#main > div:nth-child(2) > ul:nth-child(1) > li:nth-child(1) > a:nth-child(1)`

połączone z:

`#main > div:nth-child(2) > ul:nth-child(1) > li:nth-child(8) > a:nth-child(1)`

daje

`#main > div:nth-child(2) > ul:nth-child(1) > li:nth-child({x}) > a:nth-child(1)`

gdzie `{x}` oznacza dowolną liczbę. Wygnerowany selektor zwróci więc wszystkie obiekty `a` z wszystkich obiektów `li`.
