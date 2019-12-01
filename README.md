# SIFT (scale-invariant feature transform) 
Je to algoritmus z oblasti poèítaèového videnia, urèenı pre detekciu a 
popísanie základnıch rysov obrázku. Bol patentovanı Univerzitou Britskej Kolumbie v Kanade a v roku 1999 ho publikoval
 David Lowe. Tento algoritmus pozostáva z troch hlavnıch èastí a to:
1.	Detekcia k¾úèovıch bodov (*ang. keypoints detection*)
2.	Vytvorenie deskiptorov pre tieto k¾úèové body (*ang. keypoint descriptor*)
3.	Spájanie základnıch rysov obrázku (*ang. feature matching*)

### Keypoint detection
Prvım krokom tohto algoritmu je získanie k¾úèovıch bodov. Obrázok najskôr rozmaeme, 
èo nám odstráni zbytoènı šum z obrázka a tım vyniknú dôleité èasti. 
Pôvodnı obrázok zmenšíme a toto rozmazávanie opakujeme. Rozmazávaním a zmenou ve¾kosti obrázku získame tzv. 
scale space. Tieto obrázky budeme medzi sebou následne odèítava, èím získame Difference of Gaussian z ktorıch urèíme k¾úèové body. 
Tieto body nájdeme s pomocou lokálneho maxima a minima, ktoré získame porovnávaním kadého pixelu s okolitımi pixelmi a s
 pixelmi s predchádzajúcej a nasledujúcej oktávy – dokopy ho porovnáme s 26 pixelmi. Následne vyberieme stabilné k¾úèové body. 
### Keypoint descriptor
Druhá èas algoritmu sa venuje vytvoreniu deskriptorov. Pri vytváraní deskriptorov musíme najskôr 
vypoèíta gradienty pre osi x a y. Tieto vypoèítame tak, e pre os x odèítame hodnoty vpravo a v¾avo od 
poadovaného pixelu a absolútna hodnota tohto rozdielu je náš gradient pre os x. Podobnı postup opakujeme aj pre os y. 
S tımito hodnotami dokáeme vypoèíta magnitúdu a orientáciu kadého pixelu v obrázku. Z tıchto hodnôt vytvoríme histogram 
orientovanıch gradientov (ang. Histogram of Oriented Gradients). Pod¾a jeho vrcholu vieme urèi orientáciu pixelu. 
Ak je vrcholov viac, vygenerujeme ïalší k¾úèovı bod. Deskriptor vytvoríme tak, e pre kadı k¾úèovı bod vezmeme blok 
okolitıch 16x16 pixelov. Tento blok rozdelíme na 4x4 podèasti a pre kadı vytvoríme HoG. 
S tımito hodnotami dokáeme deskriptor vizualizova. 
### Feature matching
Spájanie k¾úèovıch bodov je posledná èas algoritmu SIFT. Pre dva rôzne obrázky sú vytvorené k¾úèové body a ich unikátne deskriptory. 
Pod¾a tıchto údajov vieme spoji k¾úèové body a toto spojenie zobrazi. Bıva konvenciou	obmedzi poèet zobrazenıch spojení - kvôli preh¾adnosti.
### Aplikácie SIFT algoritmu 
- rozpoznávanie objektov,
- rozpoznávanie osôb, 
- spájanie obrázkov (napr. pri vytváraní panoramatickej fotografie), 
- rozpoznávanie gest a podobne. 

### Súvisiace algoritmy:
[SURF](https://en.wikipedia.org/wiki/Speeded_up_robust_features) je jednoduchšia a rıchlejšia verzia algoritmu SIFT.
[Harris Corner Detector](https://en.wikipedia.org/wiki/Harris_Corner_Detector) slúi na extrakciu hrán a základnıch rysov obrázku.
[FAST](https://opencv-python-tutroals.readthedocs.io/en/latest/py_tutorials/py_feature2d/py_fast/py_fast.html) je algoritmus na detekciu hrán a základnıch rysov obrázku a bol vyvinutı hlavne pre pouitie v reálnom èase.

