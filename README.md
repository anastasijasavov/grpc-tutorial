Projekat iz naprednog softverskog inzenjerstva. 
# Razvoj gRPC servisa u .NET-u

#### Osnovni koncepti gRPC-ja, arhitektura i struktura jednog gRPC projekta. Tipovi komunikacije koje podrzava gRPC. Prednosti i mane

- [Sta je gRPC?](#sta-je-grpc)
- [Prednosti i mane](#prednosti-i-mane)
- [Kada koristiti gRPC?](#kada-koristiti-grpc)
- [Tipovi komunikacije (strimovanja)](#tipovi-komunikacije-strimovanja) 
- [Protocol Buffer](#protocol-buffer)
    - [Kardinalnost podataka](#kardinalnost-podataka)
    - [Brisanje polja](#brisanje-polja)
    - [Poruke](#poruke)
    - [Definisanje servisa](#definisanje-servisa)
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

# Protocol Buffer

Cross-platform format podataka koji se koristi za serijalizaciju strukturisanih podataka. Koristi IDL (interface definition language) pomocu kog se opisuje struktura poruka, i generator koda koji cita strukturu podataka koji ce da se salju/primaju i generise kod koji ce da salje/prima te strim bajtova koji predstavljaju strukturisane podatke.
Koriste se .proto fajlovi koji se sastoje od *poruka* i *servisa*. 
Za C#, kompajler generiše .cs fajl iz svakog .proto fajla sa klasom za svaki tip poruke opisan u .proto fajlu.

##Poruke 

**Poruke** se sastoje od tipa podatka i imena polja, i koriste se celi brojevi za identifikaciju svakog polja. Primer:
```
syntax = "proto3";

message SimpleRequest {
  string name = 1;
  string description = 2;
  int32 id = 3;
}
```
Na pocetku svakog proto fajla treba da se definise sintaksna verzija proto fajla, podrazumevana verzija je proto2 ukoliko se ne doda specifikacija verzije.
```syntax = "proto3";```

Svako polje mora da se sastoji od tipa polja, imena i broja. 
### Pravila za numerisanje polja
- Polje mora da ima broj od 1 do 536,870,911.
- Broj polja mora da bude unikatno medju drugim poljima.
- Brojevi izmedju 19000 i 19999 su rezervisani
- Ne smeju da se koriste prethodno rezervisana polja (brojke koje su definisane kljucnom recju **reserved** - obrisana polja).

Moguci tipovi podataka za polja su:

- **int32** (ukoliko ce vrednost da cesto dobija negativne vrednosti preporucuje se upotreba sint32 umesto int32), **float**, **double**, **int64** (isto kao i kod int32, preporucuje se sint64 ukoliko ce polje da ima cesce negativne vrednosti, enkodira negativne vrednosti efikasnije od int64), **uint32**, **uint64**, **fixed32** (sadrzi 4 bajta, efikasniji ukoliko je vrednost cesce veca od 2^28), **fixed64** (isto kao za fixed32, samo za vrednosti vece od 2^56), **sfixed32**, **sfixed64**, **bool**, **string** (UTF-8 enkodiranje, ili 7-bitni ASCII tekst, ogranicenje je da duzina bude do 2^32), **bytes** (do 2^32 duzina bajtova).

**Bool** vrednosti su po defaultu false, **bytes** je prazan, **string** je “”, numericke vrednosti imaju defaultnu vrednost 0,
Za **enume** podrazumevana vrednost je prva vrednost definisana u enumu (koja mora da ima vrednost 0), takodje prva vrednost enuma bi trebalo da se nazove <Ime_Enuma>UNSPECIFIED ili <Ime_enuma_>UNKNOWN.

Primer za enum:
```
enum ResponseType {
  RESPONSE_UNSPECIFIED = 0;
  RESPONSE_SUCCESS = 1;
  RESPONSE_ERROR = 2;
  RESPONSE_WARNING = 3;
}

message GetGalleries {
  string query = 1;
  int32 page_number = 2;
  ResponseType responseType = 3;
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
## Kardinalnost podataka

Kljucna rec *required* oznacava da je to polje obavezno, dok *optional* oznacava da je opciono da to polje ucestvuje u prenosu podataka.
Kao sto vidimo, poruka Point je deo poruke Line, i to imamo 2 polja koje su tipa Point u okviru poruke Line.
Polje takodje moze da ima tip **repeated** (moze da se ponavlja 0 ili vise puta, redosled podataka u tom nizu se takodje cuva) i 
**map** koji je pogodan za kljuc/vrednost tipove podataka.

## Brisanje polja
Brisanje polja može izazvati ozbiljne probleme ako se ne uradi kako treba.

> [!CAUTION]
> Kada vam više ne treba polje i sve reference su obrisane iz klijentskog koda, možete izbrisati definiciju polja iz poruke. 
> Međutim, morate rezervisati broj izbrisanog polja kljucnom recju *reserved*:
```
message Foo {
  reserved 2, 15, 9 to 11;
}
```
## Definisanje servisa
Servisi su na neki nacin metode u interfejsu koje definisu kako ce da se upotrebljavaju poruke. Servisi se definisu na sledeci nacin:
```
service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}
```
U ovom primeru, naziv metode koja poziva RPC se zove **SayHello**, u prvim zagradama je definisan tip ulaznih parametara koji ce da se koriste prilikom poziva te metode, a u zagradama nakon returns se definise izlazni tip poruke, tj. format odgovora od servisa.


# Kreiraj svoj gRPC servis

gRPC servis je moguce kreirati direktno iz Visual Studia, jer postoji vec templejt za takav servis. Potrebno je da imate instaliran .NET SDK
Ukoliko ne koristite Visual Studio, potrebno je uneti komandu u terminalu:
```dotnet new grpc -o <ime_projekta> ``` Nazvacemo ime projekta **GrpcTestProject**.
Potrebne su nam biblioteke koje ce nam omoguciti olaksan rad sa proto fajlovima kao sto su
**grpc.tools** - Sluzi za generisanje klasa iz odredjenih proto fajlova 
**microsoft.entityFrameworkCore.design**
Koristicemo i SQLite bazu, tako da ce nam trebati i **microsoft.entityFrameworkCore.sqlite**.

> [!CAUTION]
> Nakon svake promene .proto fajlova potrebno je pokrenuti Buildovanje projekta da bi se izgenerisale klase od definisanih proto fajlova u odgovarajucem programskom jeziku.
 
Struktura projekta
- Protos
      - greet.proto
- Services
      - GreeterService.cs
Servis:
```
using Grpc.Core;
using GrpcTestProject;

namespace GrpcTestProject.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
```
