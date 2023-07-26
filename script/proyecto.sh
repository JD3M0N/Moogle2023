
goback(){
    cd ..
    cd script
}


run(){
    echo -e "\e[32mLaunching Moogle!...\e[0m"
    cd ..
    dotnet watch run --project MoogleServer
    clear
    echo -e "\e[31mStopped Moogle! Execution.\e[0m"
    cd script
}


report(){
    echo -e "\e[32mBuilding Report...\e[0m"
    cd ..
    cd Informe
    latexmk -pdf Informe.tex
    clear
    echo -e "\e[32mBuilt Report.\e[0m"
    goback
}


slides(){
    echo -e "\e[32mBuilding Slides...\e[0m"
    cd ..
    cd Presentacion
    latexmk -pdf Presentacion.tex
    clear
    echo -e "\e[32mBuilt Slides.\e[0m"
    goback
}

show_report(){
    clear
    echo -e "\e[32mShowing Report...\e[0m"
    cd ..
    cd Informe
    
    if [ ! -f  "Informe.pdf" ];
    then 
        report;
    fi

    cd ..
    cd Informe
    if [ -z "$1" ];
    then
        echo -e "\e[33mDefault pdf viewer.\e[0m"
        xdg-open Informe.pdf
    else
        echo -e "\e[33mUser specified pdf viewer.\e[0m"
        $1 Informe.pdf
    fi
}

show_slides(){
    clear
    cd ..
    cd Presentacion

    if [ ! -f "Presentacion.pdf" ];
    then
        slides;
    fi

    cd ..
    cd Presentacion
    echo -e "\e[32mShowing Slides...\e[0m"
    if [ -z "$1" ];
    then
        echo -e "\e[33mDefault slides viewer.\e[0m"
        xdg-open Presentacion.pdf
    else
        echo -e "\e[33mUser specified slides viewer.\e[0m"
        $1 Presentacion.pdf
    fi    
}

clean(){
    echo -e "\e[33mCleaning Temporal Files...\e[0m"
    cd ..
    cd Informe
    rm Informe.aux Informe.fdb_latexmk Informe.fls Informe.log indent.log pdflatex32230.fls Informe.synctex.gz Informe.pdf
    cd ..
    cd Presentacion
    rm -f Presentacion.aux Presentacion.fdb_latexmk Presentacion.fls Presentacion.log Presentacion.nav Presentacion.snm Presentacion.toc Presentacion.out Presentacion.synctex.gz Presentacion.pdf
    cd sections
    rm -f arch.aux dataflow.aux intro.aux
    cd ..
    clear
    echo -e "\e[33mTemporal Files Cleaned.\e[0m"
    goback

}

FUNCTIONS="run report slides show_report show_slides clean"

execute=$1
# If there's no parameter, show available functions
if [ "$execute" = "" ]; then
    echo "Functions:"
    for i in $FUNCTIONS; do
        echo "-$i"
    done
    exit 1
fi

"$@"