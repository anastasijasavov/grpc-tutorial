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
gRPC je otvorenog koda i ima najveću primenu u razvoju mikroservisa, zbog brzine prenosa podataka i efikasnosti 
(strukture paketa podataka koji se brže čitaju u odnosu na klasične REST API-jeve koji koriste JSON za komunikaciju). <br>

# Prednosti i mane

## Prednosti 

- Performanse - gRPC je optimizovan za visoku brzinu i efikasnost.
Koristi binarni protokol prenosa podataka koji je manje zahtevan za mrežu u odnosu na tekstualne formate (npr. JSON).
Ovo omogućava brži prenos podataka i manje opterećenje mreže.
- Strogo tipizirana komunikacija - koriste se protobuf ugovori (contracts) za definisanje interfejsa i poruka koje se razmenjuju.
Stroga tipizacija omogucava lakše otkrivanje grešaka.
- Multipleksiranje - višestruki pozivi: gRPC podržava višestruke pozive u okviru jedne mrežne veze.
To znači da se više poziva može izvršavati istovremeno, čime se postiže veća efikasnost i manje zagušenja mreže.
- Podrška za više jezika - gRPC pruža biblioteke i generisane kodove za podržane programske jezike kao sto su: C#, Java, Python, Go itd..

## Mane

gRPC zahteva da klijenti i serveri koriste specifične biblioteke za gRPC, što može ograničiti fleksibilnost i interoperabilnost sa drugim sistemima. <br>
Takođe, gRPC zahteva podešavanje HTTP/2 protokola za komunikaciju, što može biti složeno u nekim okruženjima, i u većini slučajeva je potrebno da se koristi neki proxy (posrednik). <br>
Trenutna rešenja koje pruža gRPC su JSON Transkodiranje (.NET 7+) i gRPC-Web koji služe kao rešenja za komunikaciju web aplikacija sa gRPC-jem.  <br>

# Kada koristiti gRPC?

- Mikroservisi i skalabilnost: gRPC je idealan za razvoj mikroservisa i komunikaciju između njih, pruža visoke performanse i efikasnost prenosa podataka, kao i mogućnost višestrukih poziva. To čini gRPC odličnim izborom za skalabilne sisteme sa velikim brojem servisa koji moraju brzo da razmenjuju podatke. 
- IoT (Internet stvari): gRPC je dobar izbor za komunikaciju između IoT uređaja i backend sistema.
Zbog svojih performansi, manje opterećuje mrežu i štedi resurse uređaja. Takođe, gRPC podržava strimovanje podataka, što je korisno za praćenje i prikupljanje podataka sa IoT uređaja.


# Tipovi komunikacije (strimovanja)

gRPC podržava 4 tipa komunikacije
 - Unarni protokol udaljene procedure
<br> Metoda koja uzima 1 ulaz i vraca 1 izlazni rezultat.
 - Serversko strimovanje RPC-ja
<br> Prima 1 ulazni paket i vraća strim izlaznog rezultata. Pogodan kada se na serveru izvršavaju teže operacije koje oduzimaju više vremena.
 - Klijentsko strimovanje
<br> Otvara se konekcija ka serveru, i kada server prihvati zahtev za konekciju, klijent može da šalje podatke dok se strim ne zatvori. <br>
Ovaj tip komunikacije je pogodan kada je bitno da nema velikog kašnjenja podataka
 - Bidirekciono strimovanje
<br> Omogućava istovremeno slanje i primanje strima podataka u oba smera. Pogodan za komunikaciju u realnom vremenu.
  
![image](https://github.com/user-attachments/assets/6a88fb41-dedc-4a08-9622-c89f3c9b1a6c)

# Status kodovi

Status kodovi koje gRPC servis može da ima su:

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

Cross-platform format podataka koji se koristi za serijalizaciju strukturisanih podataka. Koristi IDL (**interface definition language**) pomoću kog se opisuje struktura poruka, i generator koda koji čita strukturu podataka koji će da se šalju/primaju i generiše kod koji ce da šalje/prima taj strim bajtova koji predstavljaju strukturisane podatke. <br>
Koriste se **.proto** fajlovi koji se sastoje od *poruka* i *servisa*.  <br>
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
Na početku svakog .proto fajla treba da se definiše sintaksna verzija proto fajla, podrazumevana verzija je proto2 ukoliko se ne doda specifikacija verzije. <br>
```syntax = "proto3";``` <br>

Svako polje mora da se sastoji od tipa polja, imena i broja.  <br>
### Pravila za numerisanje polja
- Polje mora da ima broj od 1 do 536.870.911.
- Broj polja mora da bude unikatno medju drugim poljima.
- Brojevi izmedju 19000 i 19999 su rezervisani
- Ne smeju da se koriste prethodno rezervisana polja (brojke koje su definisane ključnom rečju **reserved** - obrisana polja). <br>

Mogući tipovi podataka za polja su: <br>

- **int32** (ukoliko ce vrednost često da dobija negativne vrednosti preporučuje se upotreba sint32 umesto int32), <br> **float**, **double**, **int64** (isto kao i kod int32, preporučuje se sint64 ukoliko će polje da ima češće negativne vrednosti, enkodira negativne vrednosti efikasnije od int64), <br> **uint32**, **uint64**, **fixed32** (sadrzi 4 bajta, efikasniji ukoliko je vrednost češće veća od 2^28), <br> **fixed64** (isto kao za fixed32, samo za vrednosti vece od 2^56), <br> **sfixed32**, **sfixed64**, **bool**, **string** (UTF-8 enkodiranje, ili 7-bitni ASCII tekst, ograničenje je da dužina bude do 2^32), **bytes** (do 2^32 dužina bajtova). <br> <br>

**Bool** vrednosti su po defaultu false, **bytes** je prazan, **string** je “”, numeričke vrednosti imaju podrazumevanu vrednost 0.
Za **enume** podrazumevana vrednost je prva vrednost definisana u enumu (koja mora da ima vrednost 0), takođe prva vrednost enuma bi trebalo da se nazove **<Ime_Enuma>UNSPECIFIED** ili **<Ime_enuma_>UNKNOWN.** <br>

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
> Podaci protocol bafera sadrže samo brojeve, ne i nazive polja, pružajući određene uštede u poređenju sa sistemima koji uključuju nazive polja.

Takodje, određena poruka može da se koristi kao tip podatka za neku drugu poruku, u okviru nje, na primer:
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

Ključna rec *required* označava da je to polje obavezno, dok *optional* označava da je opciono da to polje učestvuje u prenosu podataka. <br>
Kao što vidimo, poruka Point je deo poruke Line, i to imamo 2 polja koje su tipa Point u okviru poruke Line. <br>
Polje takođe može da ima tip **repeated** (može da se ponavlja 0 ili vise puta, redosled podataka u tom nizu se takodje čuva) i 
**map** koji je pogodan za ključ/vrednost tipove podataka.<br>

## Brisanje polja
Brisanje polja može izazvati ozbiljne probleme ako se ne uradi kako treba.

> [!CAUTION]
> Kada vam više ne treba polje i sve reference su obrisane iz klijentskog koda, možete izbrisati definiciju polja iz poruke. 
> Međutim, morate rezervisati broj izbrisanog polja kljucnom rečju *reserved*:
```
message Foo {
  reserved 2, 15, 9 to 11;
}
```
## Definisanje servisa
Servisi su na neki način metode u interfejsu koje definišu kako će da se upotrebljavaju poruke. Servisi se definišu na sledeći način:
```
service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}
```
U ovom primeru, naziv metode koja poziva RPC se zove **SayHello**, u prvim zagradama je definisan tip ulaznih parametara koji će da se koriste prilikom poziva te metode, a u zagradama nakon returns se definiše izlazni tip poruke, tj. format odgovora od servisa. <br>


# Kreiraj svoj gRPC servis

gRPC servis je moguće kreirati direktno iz Visual Studia, jer postoji već templejt za takav servis. Potrebno je da imate instaliran .NET SDK. <br>
Ukoliko ne koristite Visual Studio, potrebno je uneti komandu u terminalu: <br>
```dotnet new grpc -o <ime_projekta> ``` <br>
Nazvaćemo ime projekta **GrpcTestProject**. <br>
Potrebne su nam biblioteke koje će nam omogućiti olakšan rad sa proto fajlovima kao što su <br>
 - **grpc.tools** - Služi za generisanje klasa iz određenih proto fajlova <br> 
 - **microsoft.entityFrameworkCore.design** <br>
Koristićemo i SQLite bazu, tako da će nam trebati i
 - **microsoft.entityFrameworkCore.sqlite**.
<br> Za instaliranje biblioteka možete koristiti Nuget package manager ili preko konzole ukucati komandu <br>
```dotnet add package <ime_paketa>```

> [!CAUTION]
> Nakon svake promene .proto fajlova potrebno je pokrenuti buildovanje projekta da bi se izgenerisale klase od definisanih proto fajlova u odgovarajućem programskom jeziku.
 
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
Kao što vidimo, funkcija SayHello je obična funkcija koja prima HelloRequest objekat kao ulazni parametar, taj objekat se generisao prilikom buildovanja projekta, tj u **greet.proto** fajlu smo definisali strukturu HelloRequest objekta, i potom se izgenerisala odgovarajuća klasa u C# koju možemo da koristimo za biznis logiku projekta. <br>
Takođe, neophodno je da se funkcija **predefiniše** koristeći ključnu reč *override* prilikom definisanja metode, jer je protoc (kompajler za proto fajlove) već izgenerisao metodu SayHello u C#-u i na nama je da predefinišemo logiku te metode. <br>

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

// Poruka koja se šalje prilikom poziva metode SayHello (ulazni parametar je polje name)
message HelloRequest {
  string name = 1;
}

// Sadrzaj response-a koji se vraća prilikom poziva SayHello
message HelloReply {
  string message = 1;
}
```
Ovaj primer je takođe primer jednog unarnog poziva. <br>

## Primer klijentskog streaming poziva udaljene procedure
Definicija servisa <br>

```
 rpc UpdateGalleriesPhotos(stream AddGalleryPhoto) returns (MultiGalleryResponse){}
```
Kao što vidimo, otvoriće se veza između klijenta i servera, ali će server da vrati odgovor jednom, dok klijent može da pošalje stream poziva da bi dobio 1 odgovor.
Koristimo primer dodavanja više slika u galeriji: <br>
Definicija poruke <br>

```
message AddGalleryPhoto {
    string imagePath = 1;
    string name = 2;
    int32 year = 4;
    int32 gallery_id = 5;
}
```
Vraćamo kao odgovor listu slika, uz pomoć ključne reči **repeated**.
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
Kao što vidimo, ažuriramo podatke dok klijent šalje redom pozive za ažuriranje galerije. <br> Treba imati u vidu da konekcija ne sme da se prekine, i da treba ustanoviti neki retry policy ukoliko dođe do neuspešnog zahteva zbog prekida veze. <br> Takođe greške na serveru mogu da dovedu do prekida streama i do gubitka podataka, tako da je potrebno da se validiraju poruke pre njihovog procesiranja. <br>
S druge strane, prednosti su da je moguće slati ogromne količine podataka podeljene na manje delove. <br>

## Primer server streaming poziva udaljene procedure

Definicija servisa <br>
```
service Traffic {
    rpc GetTrafficInformation(TrafficRequest) returns (stream TrafficResponse);
}
```
Kao što vidimo, treba da dobijemo stream kao odgovor sa servera. 
<br>
Dobar primer bi mogao da bude neki odgovor sa servera gde je promenljivost podataka jako česta. <br> 
U te svrhe definisali smo servis poziv koji vraća trenutnu gužvu u saobraćaju na određenog lokaciji:  <br>
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
Ovde smo definisali i enum za status gužve unutar same poruke.
<br> Tako je taj enum ostao nevidljiv za ostatak proto fajla, i ako bismo želeli da ima veći scope, morali bismo da ga definišemo van poruke. <br>
Takođe vraćamo i timestamp tip sto je ugrađena google klasa, koju smo prethodno importovali:
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
Ovde smo napravili simulaciju realnog događaja gde bismo dobijali odgovor od nekog eksternog servisa o statusu gužve na svake 3 sekunde. <br> Pre same operacije, proverili smo ukoliko je poziv otkazan od strane klijenta pomoću CancellationToken.IsCancellationRequested flaga.   <br>

## Primer dvosmernog streaming poziva udaljene procedure

Definicija servisa <br>
```
service Chat {
   rpc SendMessage(stream ClientMessage) returns (stream ServerMessage){}
}
```
U ovom slučaju mi šaljemo i primamo stream podataka. U te svrhe možemo da vidimo simulaciju jednog chata, gde nam je potrebno da veza između klijenta i servera bude otvorena, i potom šaljemo i primamo poruke od servera u isto vreme. <br>

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
Isto smo uradili i za slanje odgovora klijentu sa strane servera. <br> Imamo u oba slučaja proveru ukoliko je zatraženo da se prekine konekcija. Ovde doduše imamo neprekidno slanje odgovora sa serverske strane ka klijentu dok klijent sam ne prekine konekciju. <br>
Na samom kraju, možemo da definišemo samu funkciju koja obuhvata obe ove navedene metode <br>

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
Kao ulazne parametre za ovu metodu imamo i stream sa klijenta, i streamWriter za odgovore sa strane servera, pomoću kog možemo da šaljemo nove poruke.<br>

## Pokretanje projekta

Za pokretanje projekta, potrebno je uneti komandu ***dotnet run*** u terminalu. <br> <br>

![image](https://github.com/user-attachments/assets/764e41fe-c809-4c38-a25b-18ea72aa179c)

Server je aktivan na navedenim portovima, tako da ćemo iskoristiti HTTPS URL. <br>
Da bismo testirali i slali pozive ka kreiranom gRPC servisu, koristićemo Postman kao klijenta za naše potrebe. <br>
Postman ima ugrađenu opciju za gRPC pozive, i potrebno je kreirati novi gRPC poziv. Nakon toga pojaviće vam se prozor <br>

![image](https://github.com/user-attachments/assets/ed0c4ba6-0150-4630-b299-61ddedcb7ea7)

Nakon unosa URL-a , potrebno je uključiti TLS klikom na ikonicu sa katancem. Nakon toga treba odabrati određeni poziv <br>
Moguće je importovati proto fajl gde će Postman da prepozna sve moguće rpc pozive koje ste definisali u proto fajlu. <br>

![image](https://github.com/user-attachments/assets/00923b7e-b11f-46a7-a7ff-ab6f76dd596a)

Nakon odabira .proto fajla pojaviće se dropdown lista sa mogućim pozivima iz proto fajla <br>

![image](https://github.com/user-attachments/assets/d7160b76-4dd1-42dd-9099-5aa9ef5a4444)

Klikom na dugme Invoke, poziva se servis <br>

![image](https://github.com/user-attachments/assets/e7a20eeb-00af-46db-8b3f-3a65665cf598)


