const imgFile = document.getElementById('FotoFile');
const imgUser = document.getElementById('imgUser');


imgFile.addEventListener("change",() =>{

    const imagenes = imgFile.files;

    if (imagenes || imagenes.length) {

        const imagen = imagenes[0];
        imgUser.src = URL.createObjectURL(imagen);
    }
});