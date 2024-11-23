Projekat iz naprednog softverskog inzenjerstva. 
# Razvoj gRPC servisa u .NET-u

#### Osnovni koncepti gRPC-ja, arhitektura i struktura jednog gRPC projekta. Tipovi komunikacije koje podrzava gRPC. Prednosti i mane

- [Sta je gRPC?](#sta-je-grpc)
- [Prednosti i mane](#prednosti-i-mane)
- [Kada koristiti gRPC?](#kada-koristiti-grpc)
- [Protocol Buffer](#protocol-buffer)
- [Tipovi komunikacije (strimovanja)](#tipovi-komunikacije-strimovanja) 
- [Kreiraj svoj gRPC servis](#kreiraj-svoj-grpc-servis)

# Sta je gRPC?

gRPC (remote procedure call - poziv udaljene procedure) je framework, inicijalno kreiran od kompanije Google, koristeći Google Protocol Buffer-e.
gRPC je otvorenog koda i ima najvecu primenu u razvoju mikroservisa, zbog brzine prenosa podataka i efikasnosti 
(strukture paketa podataka koji se brze citaju u odnosu na klasicne REST API-jeve koji koriste JSON za komunikaciju).

# Prednosti i mane

## Prednosti 

- Performanse: gRPC je optimizovan za visoku brzinu i efikasnost.
Koristi binarni protokol prenosa podataka koji je manje zahtevan za mrežu u odnosu na tekstualne formate (npr. JSON).
Ovo omogućava brži prenos podataka i manje opterećenje mreže.
- Strogo tipizirana komunikacija: koriste se protobuf ugovori (contracts) za definisanje interfejsa i poruka koje da se razmenjuju.
Stroga tipizacija omogucava lakse otkrivanje gresaka.
- Multipleksiranje - visestruki pozivi: gRPC podrzava višestruke pozive u okviru jedne mrežne veze.
To znači da se više poziva može izvršavati istovremeno, čime se postiže veća efikasnost i manje zagušenja mreže.
- Podrska za vise jezika: gRPC pruža biblioteke i generisane kodove za podržane programske jezike kao sto su: C#, Java, Python, Go itd..

## Mane

gRPC zahteva da klijenti i serveri koriste specifične biblioteke za gRPC, što može ograničiti fleksibilnost i interoperabilnost sa drugim sistemima. 
Takođe, gRPC zahteva podešavanje HTTP/2 protokola za komunikaciju, što može biti složeno u nekim okruženjima, i u vecini slucajeva je potrebno da se koristi neki proxy (posrednik).
Trenutna resenja koje pruza gRPC su JSON Transkodiranje (.NET 7+) i gRPC-Web koji sluze kao resenja za komunikaciju web aplikacija sa gRPC-jem. 

# Kada koristiti gRPC?

- Mikroservisi i skalabilnost: gRPC je idealan za razvoj mikroservisa i komunikaciju između njih, pruža visoke performanse i efikasnost prenosa podataka, kao i mogućnost višestrukih poziva. To čini gRPC odličnim izborom za skalabilne sisteme sa velikim brojem servisa koji moraju brzo da razmenjuju podatke.
- IoT (Internet stvari): gRPC je dobar izbor za komunikaciju između IoT uređaja i backend sistema.
Zbog svojih performansi, manje opterećuje mrežu i štedi resurse uređaja. Takođe, gRPC podržava strimovanje podataka, što je korisno za praćenje i prikupljanje podataka sa IoT uređaja.

# Protocol Buffer

Cross-platform format podataka koji se koristi za serijalizaciju strukturisanih podataka. Koristi IDL (interface definition language) pomocu kog se opisuje struktura poruka, i generator koda koji cita strukturu podataka koji ce da se salju/primaju i generise kod koji ce da salje/prima te strim bajtova koji predstavljaju strukturisane podatke.
Koriste se .proto fajlovi koji se sastoje od *poruka* i *servisa*. 
Vrste
Poruke se sastoje od tipa podatka i imena polja, koristi cele brojeve za identifikaciju svakog polja. Primer:
```
syntax = "proto3";

message SimpleRequest {
  string name = 1;
  string description = 2;
  int32 id = 3;
}
```
> [!NOTE]
> Podaci protocol bafera sadrže samo brojeve, ne i nazive polja, pružajući određene uštede u poređenju sa sistemima koji uključuju nazive polja u podatke.

Takodje, odredjena poruka moze da se koristi kao tip podatka za neku drugu poruku, u okviru nje, na primer:
```
message Point {
  required int32 x = 1;
  required int32 y = 2;
  optional string label = 3;
}

message Line {
  required Point start = 1;
  required Point end = 2;
  optional string label = 3;
}
```
Kljucna rec *required* oznacava da je to polje obavezno, dok *optional* oznacava da je opciono da to polje ucestvuje u prenosu podataka.
Kao sto vidimo, poruka Point je deo poruke Line, i to imamo 2 polja koje su tipa Point u okviru poruke Line.

# Tipovi komunikacije (strimovanja)

gRPC podrzava 4 tipa komunikacije
- unarni protokol udaljene procedure
Metoda koja uzima 1 ulaz i vraca 1 izlazni rezultat.
- serversko strimovanje RPC-ja
Prima 1 ulazni paket i vraca strim izlaznog rezultata. Pogodan kada se na serveru izvrsavaju teze operacije koje oduzimaju vise vremena.
- klijentsko strimovanje
Otvara se konekcija ka serveru, i kada server prihvati zahtev za konekciju, klijent moze da salje podatke dok se strim ne zatvori.
Ovaj tip komunikacije je pogodan kada je bitno da nema velikog kasnjenja podataka
- bidirekciono strimovanje
Omogucava istovremeno slanje i primanje strima podataka u oba smera. Pogodan za komunikaciju u realnom vremenu.
  
![image](https://github.com/user-attachments/assets/6a88fb41-dedc-4a08-9622-c89f3c9b1a6c)

# Kreiraj svoj gRPC servis
