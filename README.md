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

gRPC (remote procedure call - protokol za udaljene procedure) je framework, inicijalno kreiran od kompanije Google, koristeći Google Protocol Buffer-e.
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


# Tipovi komunikacije (strimovanja)

gRPC podrzava 4 tipa komunikacije
- unarni protokol udaljene procedure
- serversko strimovanje RPC-ja
- klijentsko strimovanje
- bidirekciono strimovanje
![image](https://github.com/user-attachments/assets/6a88fb41-dedc-4a08-9622-c89f3c9b1a6c)

# Kreiraj svoj gRPC servis
