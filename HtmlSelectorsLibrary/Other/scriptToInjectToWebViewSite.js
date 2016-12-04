/*
 * Kopia kodu z którejś z aplikacji; przykład jak przesyłać informacje o kliniętym obiekcie w WebView do kodu c#
 */


/*Wyłącz wszystkie eventy typu mouseClick*/

document.addEventListener("click", handler, true);

function handler(e) {
    e.stopPropagation();
    e.preventDefault();

    //document.getElementById("s5_body_padding").innerHTML = "<font color='yellow'><h1>1</h1></font>";

    var target = e.target || e.srcElement,
        text = target.textContent || text.innerText;

    //document.getElementById("s5_body_padding").innerHTML = "<font color='yellow'><h1>2"+text+"</h1></font>";

    //var innerHtmlOfNearestChangedParent = "";
    //var obj = target.parentElement;
    //while (innerHtmlOfNearestChangedParent === "") {
    //    if (obj.innerHTML != obj.parentElement.innerHTML) {
    //        innerHtmlOfNearestChangedParent = obj.parentElement.innerHTML;
    //    }
    //    else {
    //        obj = obj.parentElement;
    //    }
    //}

    var innerTextOfNearestChangedParent = target.parentElement.parentElement.innerText;

    var newObject = {
        objectInnerText: text + "",
        nodeName: target.nodeName + "",
        className: target.className + "",
        idName: target.id + "",
        parentType: target.parentElement.nodeName + "",
        hrefLink: target.getAttribute("href") != undefined ? target.getAttribute("href") : "",
        specialAttribute: target.getAttribute("rozklad_jazdy_data_666")
    };

    //document.getElementById("s5_body_padding").innerHTML = "<font color='yellow'><h1>3</h1></font>";

    window.external.notify(JSON.stringify(newObject) + "");
    //window.external.notify(e.originalTarget.innerText);
    //document.getElementById("s5_body_padding").innerHTML = JSON.stringify(newObject) + "";
}