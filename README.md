# SIFT (scale-invariant feature transform) 
Je to algoritmus z oblasti po��ta�ov�ho videnia, ur�en� pre detekciu a 
pop�sanie z�kladn�ch rysov obr�zku. Bol patentovan� Univerzitou Britskej Kolumbie v Kanade a v roku 1999 ho publikoval
 David Lowe. Tento algoritmus pozost�va z troch hlavn�ch �ast� a to:
1.	Detekcia k���ov�ch bodov (*ang. keypoints detection*)
2.	Vytvorenie deskiptorov pre tieto k���ov� body (*ang. keypoint descriptor*)
3.	Sp�janie z�kladn�ch rysov obr�zku (*ang. feature matching*)

### Keypoint detection
Prv�m krokom tohto algoritmu je z�skanie k���ov�ch bodov. Obr�zok najsk�r rozma�eme, 
�o n�m odstr�ni zbyto�n� �um z obr�zka a t�m vynikn� d�le�it� �asti. 
P�vodn� obr�zok zmen��me a toto rozmaz�vanie opakujeme. Rozmaz�van�m a zmenou ve�kosti obr�zku z�skame tzv. 
scale space. Tieto obr�zky budeme medzi sebou n�sledne od��tava�, ��m z�skame Difference of Gaussian z ktor�ch ur��me k���ov� body. 
Tieto body n�jdeme s pomocou lok�lneho maxima a minima, ktor� z�skame porovn�van�m ka�d�ho pixelu s okolit�mi pixelmi a s
 pixelmi s predch�dzaj�cej a nasleduj�cej okt�vy � dokopy ho porovn�me s 26 pixelmi. N�sledne vyberieme stabiln� k���ov� body. 
### Keypoint descriptor
Druh� �as� algoritmu sa venuje vytvoreniu deskriptorov. Pri vytv�ran� deskriptorov mus�me najsk�r 
vypo��ta� gradienty pre osi x a y. Tieto vypo��tame tak, �e pre os x od��tame hodnoty vpravo a v�avo od 
po�adovan�ho pixelu a absol�tna hodnota tohto rozdielu je n� gradient pre os x. Podobn� postup opakujeme aj pre os y. 
S t�mito hodnotami dok�eme vypo��ta� magnit�du a orient�ciu ka�d�ho pixelu v obr�zku. Z t�chto hodn�t vytvor�me histogram 
orientovan�ch gradientov (ang. Histogram of Oriented Gradients). Pod�a jeho vrcholu vieme ur�i� orient�ciu pixelu. 
Ak je vrcholov viac, vygenerujeme �al�� k���ov� bod. Deskriptor vytvor�me tak, �e pre ka�d� k���ov� bod vezmeme blok 
okolit�ch 16x16 pixelov. Tento blok rozdel�me na 4x4 pod�asti a pre ka�d� vytvor�me HoG. 
S t�mito hodnotami dok�eme deskriptor vizualizova�. 
### Feature matching
Sp�janie k���ov�ch bodov je posledn� �as� algoritmu SIFT. Pre dva r�zne obr�zky s� vytvoren� k���ov� body a ich unik�tne deskriptory. 
Pod�a t�chto �dajov vieme spoji� k���ov� body a toto spojenie zobrazi�. B�va konvenciou	obmedzi� po�et zobrazen�ch spojen� - kv�li preh�adnosti.
### Aplik�cie SIFT algoritmu 
- rozpozn�vanie objektov,
- rozpozn�vanie os�b, 
- sp�janie obr�zkov (napr. pri vytv�ran� panoramatickej fotografie), 
- rozpozn�vanie gest a podobne. 

### S�visiace algoritmy:
[SURF](https://en.wikipedia.org/wiki/Speeded_up_robust_features) je jednoduch�ia a r�chlej�ia verzia algoritmu SIFT.
[Harris Corner Detector](https://en.wikipedia.org/wiki/Harris_Corner_Detector) sl��i na extrakciu hr�n a z�kladn�ch rysov obr�zku.
[FAST](https://opencv-python-tutroals.readthedocs.io/en/latest/py_tutorials/py_feature2d/py_fast/py_fast.html) je algoritmus na detekciu hr�n a z�kladn�ch rysov obr�zku a bol vyvinut� hlavne pre pou�itie v re�lnom �ase.

