<h3> Project description </h3>
<hr>
In this project we had a task to make and test (unit testing) application which would check if the written content is correct in HTML. For examples if the tag < p > is inside the tag < head > that's not correct html file. Since HTML parser gets pretty complicated because many variations could be made, the text of a project let us simplify it a little bit. 
The project followed agile principles, SOLID principles and also used some patterns. First we had to make user stories, than divide tasks between two us, and after that implement them. The sprint was every week, so on Fridays we will have meetings to present everything that we have made that week. 
If the input (file or content) is correctly written in HTML (for example < p > some content < / p >), than we will save that content in file and in our database. Next time when we access that file, we can change something, and if we do that it should appear on our terminal the difference between the last modification and the new content (similar to commits on github - red colour will signalize that we removed that line, green that we added that line).
Project text is within this pdf file ftn.res2020.mihailo_vasiljevic.virtual-ui.zad4.pdf .


# Tim16

1. Arhitektura i Dizajn

Projekat broj IV je jasno podeljen na 5 komponenti. Svaka komponenta u sebi ima odredjeni broj klasa. Cilj je napraviti sto vecu izolaciju izmedju njih i ispostovati SOLID principe. Izolacija nam je potrebna kako bismo razdvojili nas sistem na vise podsistema, razdvojili zadatke na set malih taskova, lakse isplanirali, implementirali, a samim tim i lakse istestirali komponente.
Nas komponent dijagram: 

<img src="Projekat IV - Tim16.png">

Svaka komponenta ima odredjene odgovornosti koje je razlikuju od drugih. Klijent unosi podatke, parser proverava da li je uneseni html validan i upisuje u fajl, UI Controller razmenjuje informacije i glumi medijatora, VirtualUI radi sa bazom i na kraju UI ispisuje krajnji rezultat u konzolu.

Sto se tice komunikacije izmedju komponenti, ona se odvija preko interfejsa koje neke komponente pruzaju dok ih neke koriste. Kao sto se moze videti na slici mi smo napravili da Parser pruza interfejs Clientu kako bi on mogao da koristi njegove metode. Nakon unosa, klijent salje podatke prvo na proveru u okviru svoje komponente, a potom i koristeci metodu FileParsera koja su zaduzene za otvaranje/kreiranje novog fajla. Ukoliko je unos dobar, UI Controller (koji poseduje svoj interfejs IController) takodje koristi IFileParser i u okviru svog konstruktora sa parametrom prosledjuje ga kao argument. UI Controller na taj nacin smesta u svoje propertije informacije o prosledjenom fajlu. On takodje pruza usluge VirtualUI i UI. VirtualUI pri instanciranju prosledjuje kao argument svog konstruktora IController. Na taj nacin moze da uzme vrednosti iz njegovih propertija, da informacije isparsira i ubaci u bazu. 

VirtualUI jedini ima mogucnost manipulacije sa bazom. Koriscena je Entity-Framework baza podataka, gde je glavni interfejs IDBManager koji sadrzi metode za rad sa bazom. Nakon sto proveri u bazi da li fajl postoji, VirtualUI vraca informacije o Delti Controlleru tako sto se nakaci na njegovu metodu SendDeltaInformation u vec prosledjenom interfejsu IController. Na taj nacin omogucavamo Controlleru da popuni i ostale propertije koje su mu potrebni kako bi dostavio informacije UI komponenti.  Nakon toga, UI komponenti se prosledjuje isti IController interfejs. Ona zatim uzima preostale informacije o delti preko propertija tog interfejsa i ispisuje ih na konzolu.

Sto se tice dizajn paterna, koriscen je Singleton za DBManager klasu koja implementira IDBManager. Uglavnom su ispostavani SOLID principi pa jedna klasa ima jednu odgovornost, nema nepotrebnih nasledjivanja metoda kod interfejsa, izvedena klasa moze da zameni baznu itd.




2. Opis Implementacije

Sto se tice Client Component-e u njoj smo napravili poseban interfejs IValidation cije metode su zaduzene da sprece nedozvoljeni unos od strane korisnika. Taj interfejs implementira klasa Validation, pa se tako u njoj nalaze metode koje proveravaju da li je unos prazan, da li postoji fajl sa datom putanjom, da li su upisani karakteri dozvoljeni za ime fajla...Pored toga, klasa Client nam sluzi samo da pozove klasu Menu, u kojoj se nalazi sva logika klijentske komponente. Tu prvo sprecavamo korisnika da unese bilo sta osim 1 (Insert File Path), 2 (Write new File), ili ukoliko zeli da napusti aplikaziju x(X). Ukoliko je izabrana opcija 1, klijentu ce biti ponudjenu da ukuca naziv vec postojeceg fajla. Pogresan unos ce ga vratiti na pocetak. 

Ukoliko je validacija uspesno predjena, istancira se interfejs IFileParser. Parser samim tim daje interfejs na koriscenje, stoga klijent ima mogucnost da koristi njegove metode. Parser je izdeljen na 2 interfejsa – IFileParser i IParser. IFileParser sadrzi metode za otvaranje postojeceg fajla (kao i citanje teksta iz fajla) i pravljenje novog fajla, dok sa druge strane IParser sadrzi metode koje su potrebne kako bi se utvrdilo da li je upisani sadrzaj html ispravan.  Pozivanjem metode OpenExistingFile i prosledjivanjem naziva fajla mi otvaramo fajl na toj lokaciji i proveravamo sadrzaj. Nakon sto se iscita sadrzaj fajla, cuva se u stringu koji se prosledjuje IParser metodama koje sluze za proveru validnosti html unosa. Logika u IParseru je sledeca – delimo tekst na dva dela, ono sto je unutar body taga i ono sto je izvan. Kod provera izvan body taga imamo 3 provere. Prva je da li html dokument pocinje sa < html >< head >< title >. Trimuje whitespaceove za slucaj da je korisnik unosio razmake unutar i van tagova. CheckHtmlStartTagsUntilTitle je metoda koja to odradjuje. Ukoliko je sve u redu, proverava se da li su tagovi posle < title > sledeci < / title >< / head >< body >. Metoda koja ovo proverava je CheckHtmlTagsAfterTitleUntilBody. Ukoliko je i to uspesno odradjeno, proverava se taguje na kraju html dokumenta tj da li se zavrsava sa < / body >< /html >. Metoda zaduzena za proveru je CheckHtmlTagsAfterBodyUntilEnd. 

Nakon toga, prelazi se na provere unutar body taga. CheckHtmlTagsInsideBody proverava da li su tagovi unutar body taga validni. Ona poziva CheckIfValid tag koja proverava listu tagova da li su dobri, ta metoda je najvaznija. Ona takodje proverava da li su zadovoljena sledeca pravila – broj otvorenih i zatvorenih tagova mora biti isti(broj otvorenih < p > tagova mora biti jednak broju zatvorenih "< / p >"), prvi tag na koji se naidje u body tagu mora biti otvoren, i ne smeju biti dva ista taga uzastopno otvorena (< p > < p >), takodje ne sme biti ni dva uzastopna zatvorena taga (< / p > < / p >). 
Takodje imamo i metodu SplitHtmlText koja vraca listu tagova u fajlu. Ona splituje text po < i >. AllowedHtmlTags metoda nam samo ispisuje sve dozvoljene tagove. 

Opcija broj 2 je vrlo slicna s tim sto cete pre svega morati da date ime i sadrzaj koji ce biti ispisan u jednom redu. Nakon toga, on se splituje kako bi se napravili razmaci nakon svakog reda i takav sadrzaj se kreira i upisuje u novi fajl. 

Ukoliko su sve provere predjene, UI Controller instancira i prosledjuje kao argument IFileParser. Na taj nacin on moze u svoje propertije parsiranjem da smesti odgovarajuce vrednosti. Kada se kreiranje objekta zavrsi, VirtualUI pravi instancu u kojoj ce kao argument proslediti IController. Virtual UI poseduje vise klasa. Pre svega napravljen IDBManager glavni interfejs koji poseduje metode potrebne za rad sa bazom. Njega implementira DBManager koji je napravljen kao Singleton.Takodje tu su jos i interfejsi Controllera za svaku klasu iz foldera Models koji omogucavaju laksu manipulaciju sa bazom. Samim tim neke od metoda u DeltaControlleru – DeltaExists, UpdateDelta, AddDelta. Slicne metode se nalaze i u FileController (FileExist,Add) i FileContentController(Add,GetContent,GetContentbyFileId,UpdateFileContent). Pri instanciranju klasa koja koje implementiraju ove interfejse, u praznom konstruktoru prosledjujemo im Singleton – DBManager.Instance.
Za potrebe testiranja napravljen je i FakeDBManager koji imitira metode iz DBManagera i mapira ih u listu kako bismo im lakse i brze pristupali. U folderu Access osim IDBManagera nalazi se i konfiguracija koja omogucava automatske migracije tj promene u bazi. Sto se tice baze, prvo smo napravili mdf file i dodali novi sql query. Dodavanjem querija napravile su se tabele u nasoj mdf bazi (Data Connections). Potom smo pri pravljenju nove entity framework baze izabrali da uvezemo tabele iz vec postojece. Na taj nacin smo dobili klase koje smo smestili u folder Models. 

VirtualUI prima informacije od UIControllera i parsira ih posebnom petodom ParseFileInformation. Tu se instancira i dodeljuje vrednost objektima File i FileContent. Nakon toga, pravi se klasa UpdatingDatabase kako bi se proverilo da li dati fajl vec postoji u bazi. Da bismo ispostovali Single Responsibility princip napravili smo novu klasu u kojoj cemo po potrebi raditi Add/Update database. Provera da li fajl postoji u bazi ce nam odrediti da li cemo mi taj File/FileContent dodavati ili modifikovati. Ukoliko fajl nije upisan u bazu,  samim tim nema nikakve razlike izmedju sadrzaja i dodaje se u bazu. 
Ukoliko je fajl vec upisan u bazu, to znaci da postoji mogucnost da se desila neka promena u sadrzaju. Preko FileContentControllera i njegove metode GetContent dobavljamo sadrzaj(FileContent) za dati File Id. Proveravamo da li je isti sa novim sadrzajem, ukoliko jeste, nema nikakvih promena. Ako je doslo do nekih promena, instanciramo novu klasu CompareFiles gde cemo porediti date promene.
 U klasi CompareFiles mi prosledjujemo IDeltaController. Compare metoda nam sluzi da proveri sadrzaje. I novi i stari sadrzaj delimo po novom redu. Dobicemo listu stringova i potrebno je da proverimo koji sadrzaj ima vise redova. Potom iteriramo kroz niz i svaki clan niza tih stringova poredimo. Da bismo sprecili da index ode izvan opsega postavljamo jednostavne If uslove. Dodajemo novu promenljivu koju smo nazvali row i nju cemo smestati u line range od delte. Uz row u line range dodajemo i zareze. U delta content cemo prilepiti i \n kao oznaku za novi red i otvaranje mogucnosti da kasnije po tome splitujemo. Change se menja na true i to znaci da je doslo do promene i da bi trebalo da se delta ubaci ili modifikuje u bazi. Na kraju vracamo deltu. 
S obzirom da metoda Compare vraca deltu, mi proveravamo da li je delta i dalje null, ukoliko jeste, to znaci da nema nikakvih promena. Ukoliko je ipak do promena doslo, instanciramo novu klasu SendDeltaInformation kojoj cemo poslati ove nove informacije. To je ujedno i metoda IControllera, tako da u UIController klasi u propertije stavljamo vrednosti poslate sa VirtualUI tj iz SendDeltaInformation klase. Tu se zavrsava posao VirtualUI-a.

Instanciranjem UI i prosledjivanjem IControllera kao argumenta, mi mozemo da pristupimo propertijima tog interfejsa. UI klasa to i cini – prvo provera da li je mozda neki od propertija null (pre svega LineRange). To bi znacilo da nije doslo do promena, a samim tim nije ni poslat SendDeltaInformation, pa bi instanciranjem UI klase u klijentu sa null propertijima IControllera znacilo da ce nam program izbaciti neki Exception. Ukoliko je do promene pak doslo, preuzimamo vrednosti iz LineRange. Na taj nacin mi konvertujemo do tada string sa zarezima koji su oznacavali redove kod kojih je doslo do promena u niz intigera. Zatim kod klijentske komponente pozivamo klasu AddColor. Pre nego sto je pozovemo gledamo da li je LineNumbers==null kako bismo predupredili moguci exception. 
AddColor splituje prethodne vrednosti koje smo smestili u propertije IControllera. Naravno, deli se po novom redu i smesta se u niz stringova. Logika je slicna onoj pri trazenju delte. Poziva se nova metoda AddDifference kojoj cemo proslediti nizove stringova koje smo prethodno dobili kao i najduzi sadrzaj i broj redova kod kojih ima odredjenih promena. Krecemo od 1 kroz for petlju jer nam redovi idu od broja 1. Proveravamo da li data linija (koja sadrzi brojeve kod kojih je promena) sadrzi index, ukoliko sadrzi onda poredimo kako bismo znali koju boju treba da stavimo. Kako bismo izbegli index out of range, postavljamo if uslov. S obzirom da je na tom indexu doslo do promene, to znaci da ce stari sadrzaj u tom redu morati biti izbrisan, stoga bojimo u crveno. Pri bojenju mi uzimamo element koji se nalazi na databaseText[i-1] mestu jer inicijalno krecemo od jedinice. Boju smo regulisali preko ConsoleColor.Red. Ako je delta[y] – y je inicijalizovano na 0, jednako praznom stringu to znaci da je nekada bio sadrzaj koji je sada u novom tekstu obrisan stoga mi samo vracamo boju na belu i prelazimo povecavamo brojac da bi se gledala vrednost sledeceg elementa u delti. Ukoliko nije prazan string onda to znaci da je stari sadrzaj na toj liniji prelepljen novim redom te ga bojimo u zeleno. 
Na kraju ukoliko nase linije ne sadrze taj index to znaci da u tom redu nije doslo do promena. 




3. Nacin pokretanja i instaliranja

Kako bismo pravilno pokrenuli projekat, potrebno je prvo updejtovati bazu. Odlaskom na package manager console i biranjem pod DefaultProject -> VirtualUI, potrebno je ukucati u konzolu update-database. Na taj nacin u SQL ServerExploreru sa leve strane bi trebalo da se prikaze baza pod imenom FileDatabase. Tu ce nam biti smesteni svi podaci iz Modelsa (File, FileContent, Delta).
Takodje, s obzirom da u projektu postoje odredjene lokalne putanje, potrebno ih je izmeniti po potrebi. U parser komponenti – klasa FileParser u metodama CreateNewFileForParsing i OpenExistingFileForParsing potrebno je dodati lokaciju gde cuvate vas fajl. Takodje, izmena se mora napraviti i u Validation klasi na klijentskoj komponenti u metodi CheckIfPathCorrect.
 Pokretanjem klijenta, i upisivanjem ispravnih vrednosti u konzolu redno se poziva jedna po jedna komponenta. Prvo Parser, pa UIController, pa VirtualUI i na kraju UI.
