Projekat iz naprednog softverskog inzenjerstva. 
# Razvoj gRPC servisa u .NET-u

#### Osnovni koncepti gRPC-ja, arhitektura i struktura jednog gRPC projekta. Tipovi komunikacije koje podrzava gRPC. Prednosti i mane

- [Sta je gRPC?](#sta-je-grpc)
- [Prednosti i mane](#prednosti-i-mane)
- [Kada koristiti gRPC?](#kada-koristiti-grpc)
- [Tipovi komunikacije (strimovanja)](#tipovi-komunikacije-strimovanja)
- [Status kodovi](#status-kodovi)
- [Protocol Buffer](#protocol-buffer)
    - [Kardinalnost podataka](#kardinalnost-podataka)
    - [Brisanje polja](#brisanje-polja)
    - [Poruke](#poruke)
    - [Definisanje servisa](#definisanje-servisa)
- [Kreiraj svoj gRPC servis](#kreiraj-svoj-grpc-servis)
    - [Pokretanje projekta](#pokretanje-projekta)

# Sta je gRPC?

gRPC (remote procedure call - poziv udaljene procedure) je framework, inicijalno kreiran od kompanije Google, koristeći Google Protocol Buffer-e. <br>
gRPC je otvorenog koda i ima najvecu primenu u razvoju mikroservisa, zbog brzine prenosa podataka i efikasnosti 
(strukture paketa podataka koji se brze citaju u odnosu na klasicne REST API-jeve koji koriste JSON za komunikaciju). <br>

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

gRPC zahteva da klijenti i serveri koriste specifične biblioteke za gRPC, što može ograničiti fleksibilnost i interoperabilnost sa drugim sistemima. <br>
Takođe, gRPC zahteva podešavanje HTTP/2 protokola za komunikaciju, što može biti složeno u nekim okruženjima, i u vecini slucajeva je potrebno da se koristi neki proxy (posrednik). <br>
Trenutna resenja koje pruza gRPC su JSON Transkodiranje (.NET 7+) i gRPC-Web koji sluze kao resenja za komunikaciju web aplikacija sa gRPC-jem.  <br>

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

# Status kodovi
Status kodovi koje gRPC servis moze da ima su:
| Kod                 | Id | Opis                                                                                |
|---------------------|----|-------------------------------------------------------------------------------------|
| OK                  | 0  | Uspeh                                                                               |
| CANCELLED           | 1  | Operacija je otkazana, obicno od strane korisnika                                   |
| UNKNOWN             | 2  | Nepoznata greska.                                                                   |
| INVALID_ARGUMENT    | 3  | Klijent je poslao nevalidne argumente                                               |
| DEADLINE_EXCEEDED   | 4  | Rok je istekao pre nego da se operacija zavrsi.                                     |
| NOT_FOUND           | 5  | Entitet nije pronadjen.                                                             |
| ALREADY_EXISTS      | 6  | Entitet kog je klijent pokusao da kreira vec postoji.                               |
| PERMISSION_DENIED   | 7  | Klijent nema permisije da izvrsi odredjenu operaciju.                               |
| RESOURCE_EXHAUSTED  | 8  | Resurs je potrosen, moguce zbog nedostatka memorije.                                |
| FAILED_PRECONDITION | 9  | Operacija je odbijena zato sto sistem nije u neophodnom stanju da izvrsi operaciju. |
| ABORTED             | 10 | Operacija je prekinuta najverovatnije zbog problema paralelnosti.                   |
| OUT_OF_RANGE        | 11 | Operacija je van opsega.                                                            |
| UNIMPLEMENTED       | 12 | Operacija nije implementirana ili podrzana od strane odredjenog uredjaja.           |
| INTERNAL            | 13 | Serverska greska.                                                                   |
| UNAVAILABLE         | 14 | Servis je trenutno nedostupan.                                                      |
| DATA_LOSS           | 15 | Nepovratni gubitak podataka ili korupcija (ostecenje) podataka.                     |
| UNAUTHENTICATED     | 16 | Zahtev nema validne kredencijale za autentifikaciju.                                |

# Protocol Buffer

Cross-platform format podataka koji se koristi za serijalizaciju strukturisanih podataka. Koristi IDL (interface definition language) pomocu kog se opisuje struktura poruka, i generator koda koji cita strukturu podataka koji ce da se salju/primaju i generise kod koji ce da salje/prima te strim bajtova koji predstavljaju strukturisane podatke. <br>
Koriste se .proto fajlovi koji se sastoje od *poruka* i *servisa*.  <br>
Za C#, kompajler generiše .cs fajl iz svakog .proto fajla sa klasom za svaki tip poruke opisan u .proto fajlu. <br>

## Poruke 

**Poruke** se sastoje od tipa podatka i imena polja, i koriste se celi brojevi za identifikaciju svakog polja. Primer:
```
syntax = "proto3";

message SimpleRequest {
  string name = 1;
  string description = 2;
  int32 id = 3;
}
```
Na pocetku svakog proto fajla treba da se definise sintaksna verzija proto fajla, podrazumevana verzija je proto2 ukoliko se ne doda specifikacija verzije. <br>
```syntax = "proto3";``` <br>

Svako polje mora da se sastoji od tipa polja, imena i broja.  <br>
### Pravila za numerisanje polja
- Polje mora da ima broj od 1 do 536,870,911.
- Broj polja mora da bude unikatno medju drugim poljima.
- Brojevi izmedju 19000 i 19999 su rezervisani
- Ne smeju da se koriste prethodno rezervisana polja (brojke koje su definisane kljucnom recju **reserved** - obrisana polja). <br>

Moguci tipovi podataka za polja su: <br>

- **int32** (ukoliko ce vrednost da cesto dobija negativne vrednosti preporucuje se upotreba sint32 umesto int32), <br> **float**, **double**, **int64** (isto kao i kod int32, preporucuje se sint64 ukoliko ce polje da ima cesce negativne vrednosti, enkodira negativne vrednosti efikasnije od int64), <br> **uint32**, **uint64**, **fixed32** (sadrzi 4 bajta, efikasniji ukoliko je vrednost cesce veca od 2^28), <br> **fixed64** (isto kao za fixed32, samo za vrednosti vece od 2^56), <br> **sfixed32**, **sfixed64**, **bool**, **string** (UTF-8 enkodiranje, ili 7-bitni ASCII tekst, ogranicenje je da duzina bude do 2^32), **bytes** (do 2^32 duzina bajtova). <br> <br>

**Bool** vrednosti su po defaultu false, **bytes** je prazan, **string** je “”, numericke vrednosti imaju defaultnu vrednost 0.
Za **enume** podrazumevana vrednost je prva vrednost definisana u enumu (koja mora da ima vrednost 0), takodje prva vrednost enuma bi trebalo da se nazove **<Ime_Enuma>UNSPECIFIED** ili **<Ime_enuma_>UNKNOWN.** <br>

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

Kljucna rec *required* oznacava da je to polje obavezno, dok *optional* oznacava da je opciono da to polje ucestvuje u prenosu podataka. <br>
Kao sto vidimo, poruka Point je deo poruke Line, i to imamo 2 polja koje su tipa Point u okviru poruke Line. <br>
Polje takodje moze da ima tip **repeated** (moze da se ponavlja 0 ili vise puta, redosled podataka u tom nizu se takodje cuva) i 
**map** koji je pogodan za kljuc/vrednost tipove podataka.<br>

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
U ovom primeru, naziv metode koja poziva RPC se zove **SayHello**, u prvim zagradama je definisan tip ulaznih parametara koji ce da se koriste prilikom poziva te metode, a u zagradama nakon returns se definise izlazni tip poruke, tj. format odgovora od servisa. <br>


# Kreiraj svoj gRPC servis

gRPC servis je moguce kreirati direktno iz Visual Studia, jer postoji vec templejt za takav servis. Potrebno je da imate instaliran .NET SDK. <br>
Ukoliko ne koristite Visual Studio, potrebno je uneti komandu u terminalu: <br>
```dotnet new grpc -o <ime_projekta> ``` <br>
Nazvacemo ime projekta **GrpcTestProject**. <br>
Potrebne su nam biblioteke koje ce nam omoguciti olaksan rad sa proto fajlovima kao sto su <br>
 - **grpc.tools** - Sluzi za generisanje klasa iz odredjenih proto fajlova <br> 
 - **microsoft.entityFrameworkCore.design** <br>
Koristicemo i SQLite bazu, tako da ce nam trebati i
 - **microsoft.entityFrameworkCore.sqlite**.
<br> Za instaliranje biblioteka mozete koristiti Nuget package manager ili preko konzole ukucati komandu <br>
```dotnet add package <ime_paketa>```

> [!CAUTION]
> Nakon svake promene .proto fajlova potrebno je pokrenuti buildovanje projekta da bi se izgenerisale klase od definisanih proto fajlova u odgovarajucem programskom jeziku.
 
Struktura projekta
- Protos <br>
      - greet.proto <br>
- Services <br>
      - GreeterService.cs <br>
Servis koji se kreirao:
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
Kao sto vidimo, funkcija SayHello je obicna funkcija koja prima HelloRequest objekat kao input parametar, taj objekat se generisao prilikom buildovanja projekta, tj u **greet.proto** fajlu smo definisali strukturu HelloRequest objekta, i potom se izgenerisala odgovarajuca klasa u C# koju mozemo da koristimo za biznis logiku projekta. <br>
Takodje, neophodno je da se funkcija **predefinise** koristeci kljucnu rec *override* prilikom definisanja metode, jer je protoc (kompajler za proto fajlove) vec izgenerisao metodu SayHello u C#-u i na nama je da predefinisemo logiku te metode. <br>

**greet.proto** fajl:

```
syntax = "proto3";

option csharp_namespace = "GrpcTestProject";

package greet;

// Definicija metode SayHello
service Greeter {
  // Sends a greeting
  rpc SayHello (HelloRequest) returns (HelloReply);
}

// Poruka koja se salje prilikom poziva metode SayHello (input parametar je polje name)
message HelloRequest {
  string name = 1;
}

// Sadrzaj response-a koji se vraca prilikom poziva SayHello
message HelloReply {
  string message = 1;
}
```
Ovaj primer je takodje primer jednog unarnog poziva. <br>

## Primer klijentskog streaming poziva udaljene procedure
Definicija servisa <br>

```
 rpc UpdateGalleriesPhotos(stream AddGalleryPhoto) returns (MultiGalleryResponse){}
```
Kao sto vidimo, otvorice se konekcija izmedju klijenta i servera, ali ce server da vrati odgovor jednom, dok klijent moze da posalje stream poziva da bi dobio 1 odgovor.
Koristimo primer dodavanja vise slika u galeriji: <br>
Definicija poruke <br>

```
message AddGalleryPhoto {
    string imagePath = 1;
    string name = 2;
    int32 year = 4;
    int32 gallery_id = 5;
}
```
Vracamo kao odgovor listu slika, uz pomoc kljucne reci **repeated**.
```
message MultiGalleryResponse {
    repeated UpdateGalleryResponse gallery_response = 1;
}
```
Implementacija biznis logike u C# servisu <br>
```
 public override async Task<MultiGalleryResponse> UpdateGalleriesPhotos
 (IAsyncStreamReader<AddGalleryPhoto> requestStream,
  ServerCallContext context)
 {

     var response = new MultiGalleryResponse
     {
         GalleryResponse = { }
     };
     await foreach (var request in requestStream.ReadAllAsync())
     {
         var image = new Models.Photo
         {
             ImagePath = request.ImagePath,
             Year = request.Year,
             Name = request.Name,
             GalleryId = request.GalleryId

         };
         _context.Photos.Add(image);
         await _context.SaveChangesAsync();
         response.GalleryResponse.Add(new UpdateGalleryResponse { Id = image.Id });
     }
     return response;
 }
```
Kao sto vidimo, azuriramo podatke dok klijent salje redom pozive za azuriranje galerije. <br> Treba imati u vidu da konekcija ne sme da se prekine, i da treba ustanoviti neki retry policy ukoliko dodje do neuspesnog zahteva zbog prekida veze. <br> Takodje greske na serveru mogu da dovedu do prekida streama,  i da dovedu do gubitka podataka, tako da je potrebno da se validiraju poruke pre njihovog procesiranja. <br>
S druge strane, prednosti su da je moguce slati ogromne kolicine podataka podeljene na manje delove. <br>

## Primer server streaming poziva udaljene procedure

Definicija servisa <br>
```
service Traffic {
    rpc GetTrafficInformation(TrafficRequest) returns (stream TrafficResponse);
}
```
Kao sto vidimo, treba da dobijemo stream kao odgovor sa servera. 
<br>
Dobar primer bi mogao da bude neki odgovor sa servera gde je promenljivost podataka jako cesta. <br> 
U te svrhe definisali smo servis poziv koji vraca trenutnu guzvu u saobracaju na odredjenog lokaciji:  <br>
Definicija poruke <br>
```

message TrafficRequest {
    int32 location_id = 1;
}

message TrafficResponse {
    enum TrafficStatus {
            Traffic_UNSPECIFIED = 0;
            Traffic_CLEAR = 1;
            Traffic_MODERATE = 2;
            Traffic_SEVERE = 3;
    }
    TrafficStatus trafficStatus = 1;
    google.protobuf.Timestamp timestamp = 2;
    string note = 3;
}

```
Ovde smo definisali i enum za status guzve unutar same poruke.
<br> Tako je taj enum ostao nevidljiv za ostatak proto fajla, tako da ako bismo zeleli da ima veci scope, morali bismo da ga definisemo van poruke. <br>
Takodje vracamo i timestamp tip sto je ugradjena google klasa, koju smo prethodno importovali:
```
import "google/protobuf/timestamp.proto";
```
Implementacija biznis logike u C# servisu <br>
```
  public override async Task GetTrafficInformation
      (TrafficRequest request,
       IServerStreamWriter<TrafficResponse> responseStream,
       ServerCallContext context)
  {

      for (int i = 0; i < 30; i++)
      {
          if (context.CancellationToken.IsCancellationRequested)
          {
              _logger.LogInformation("The request was forcibly cancelled.");
              break;
          }
          var traffic = await _context.Traffic
              .FirstOrDefaultAsync(x => x.LocationId == request.LocationId);

          await responseStream.WriteAsync(new TrafficResponse
          {
              TrafficStatus = traffic!.TrafficStatus,
              Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
              Note = traffic.Note
          });
          await Task.Delay(3000);
      }
  }
```
Ovde smo napravili simulaciju realnog dogadjaja gde bismo dobijali odgovor od nekog eksternog servisa o statusu guzve na svake 3 sekunde. <br> Pre same operacije, proverili smo ukoliko je poziv otkazan od strane klijenta pomocu CancellationToken.IsCancellationRequested flaga.   <br>

## Primer dvosmernog streaming poziva udaljene procedure
Definicija servisa <br>
```
service Chat {
   rpc SendMessage(stream ClientMessage) returns (stream ServerMessage){}
}
```
U ovom slucaju mi saljemo i primamo stream podataka. U te svrhe mozemo da vidimo simulaciju jednog chata, gde nam je potrebno da veza izmedju klijenta i sevrera bude otvorena, i potom saljemo i primamo poruke od servera u isto vreme. <br>

Definicija poruke <br>
```
message ClientMessage {
   string text = 1; 
}

message ServerMessage {
   string text = 1; 
   google.protobuf.Timestamp timestamp = 2;
}
```

Implementacija biznis logike u C# servisu <br>

```
  private async Task HandleClientRequest(IAsyncStreamReader<ClientMessage> requestStream, ServerCallContext context)
  {
      while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
      {
          var message = requestStream.Current;
          _logger.LogInformation($"Client said {message.Text}");
      }
  }
```
Napravili smo manju metodu koja ce da izvrsava samo primanje poruka sa klijentske strane. 
```

  private static async Task<int> HandleServerResponse(IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
  {
      var pingCount = 0;

      while (!context.CancellationToken.IsCancellationRequested)
      {
          await responseStream.WriteAsync(
          new ServerMessage
          {
              Text = $"Server said hi {++pingCount} times.",
              Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
          });

          await Task.Delay(1000);
      }

      return pingCount;
  }
```
Isto smo uradili i za slanje odgovora klijentu sa strane servera. <br> Imamo u oba slucaja proveru ukoliko je zatrazeno da se prekine konekcija. Ovde doduse imamo neprekidno slanje odgovora sa serverske strane ka klijentu dok klijent sam ne prekine konekciju. <br>
Na samom kraju, mozemo da definisemo samu funkciju koja obuhvata obe ove navedene metode <br>

```
  public override async  Task SendMessage 
      (IAsyncStreamReader<ClientMessage> requestStream, 
      IServerStreamWriter<ServerMessage> responseStream, 
      ServerCallContext context)
  {
      var clientToServerTask = HandleClientRequest(requestStream, context);
      var serverToClientTask = HandleServerResponse(responseStream, context);
      
      await Task.WhenAll(clientToServerTask, serverToClientTask);
  }
```
Kao ulazne parametre za ovu metodu imamo i stream sa klijenta, i streamWriter za odgovore sa strane servera, pomocu kog mozemo da saljemo nove poruke.<br>

## Pokretanje projekta

Za pokretanje projekta, potrebno je uneti komandu ***dotnet run*** u terminalu. <br>
![image](https://github.com/user-attachments/assets/764e41fe-c809-4c38-a25b-18ea72aa179c)
Server je aktivan na navedenim portovima, tako da cemo iskoristiti HTTPS URL. <br>
Da bismo testirali i slali pozive ka kreiranom gRPC servisu, koristicemo Postman kao klijenta za nase potrebe. <br>
Postman ima ugradjenu opciju za gRPC pozive, i potrebno je kreirati novi gRPC poziv. Nakon toga pojavice vam se prozor
![image](https://github.com/user-attachments/assets/ed0c4ba6-0150-4630-b299-61ddedcb7ea7)
Nakon unosa URL-a , potrebno je ukljuciti TLS klikom na ikonicu sa katancem. Nakon toga treba odabrati odredjeni poziv <br>
Moguce je importovati proto fajl gde ce postman da prepozna sve moguce rpc pozive koje ste definisali u proto fajlu. <br>
![image](https://github.com/user-attachments/assets/00923b7e-b11f-46a7-a7ff-ab6f76dd596a)
Nakon odabira .proto fajla pojavice se dropdown lista sa mogucim pozivima iz proto fajla 
![image](https://github.com/user-attachments/assets/d7160b76-4dd1-42dd-9099-5aa9ef5a4444)
Klikom na dugme Invoke, pozvace se servis
![image](https://github.com/user-attachments/assets/e7a20eeb-00af-46db-8b3f-3a65665cf598)


