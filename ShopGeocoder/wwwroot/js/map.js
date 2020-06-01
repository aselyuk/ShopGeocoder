
var myMap;
var currentItem;
var stack;

ymaps.ready(init);

function init() {
    // Создание карты.    
    var baseCoords = [50.623287, 36.590610];

    myMap = new ymaps.Map("map", {
        center: baseCoords,
        zoom: 18,
        behaviors: ["drag", "scrollZoom", "rightMouseButtonMagnifier"]
    });

    myMap.setType('yandex#map');

    // Обработка события, возникающего при щелчке
    // правой кнопки мыши в любой точке карты.
    // При возникновении такого события покажем всплывающую подсказку
    // в точке щелчка.
    myMap.events.add('contextmenu', function (e) {
        myMap.hint.open(e.get('coords'), 'Кто-то щелкнул правой кнопкой');
    });

    // Скрываем хинт при открытии балуна.
    myMap.events.add('balloonopen', function (e) {
        myMap.hint.close();
    });

    // Обработка события, возникающего при щелчке
    // левой кнопкой мыши в любой точке карты.
    // При возникновении такого события откроем балун.
    myMap.events.add('dblclick', function (e) {
        let coords = e.get('coords');
        coords[0] = coords[0].toPrecision(10);
        coords[1] = coords[1].toPrecision(10);

        ymaps.geocode(coords, { results: 1 }).then(function (res) {
            let firstGeoObject = res.geoObjects.get(0);
            let address = firstGeoObject.getAddressLine();
            let note = [
                // Название населенного пункта или вышестоящее административно-территориальное образование.
                firstGeoObject.getLocalities().length ? firstGeoObject.getLocalities() : firstGeoObject.getAdministrativeAreas(),
                // Получаем путь до топонима, если метод вернул null, запрашиваем наименование здания.
                firstGeoObject.getThoroughfare() || firstGeoObject.getPremise()
            ].filter(Boolean).join(', ');

            if (!myMap.balloon.isOpen()) {
                myMap.balloon.open(coords, {
                    contentHeader: address,
                    contentBody: '<p>' + note + '</p>' +
                        '<h5>Координаты: <span class="badge badge-light">' + coords[0] + '</span>, ' +
                        '<span class= "badge badge-light">' + coords[1] + '</span></h5>',
                    contentFooter: '<button class="btn btn-sm btn-outline-primary" onclick="setCoords([' + coords + ']);">Сохранить эти координаты</button>'
                });
            }
            else {
                myMap.balloon.close();
            }
        });
    });
}

function setCoords(coords) {
    var stack = $("#stack");
    myMap.balloon.close();
    var item = $(self.currentItem);
    var itemId = item.attr("id");
    var data = { id: itemId, lat: coords[0], lon: coords[1] };

    var latVal = coords[0];
    var lonVal = coords[1];

    console.debug(data);

    $.ajax({
        url: "/Geocoder/UpdateCoords",
        type: "POST",
        data: data,
        success: function (data) {
            var coordsVal = item.find("#coords_" + itemId);
            var lat = coordsVal.find("#lat_" + itemId);
            lat.text(coords[0]);
            var lon = coordsVal.find("#lon_" + itemId);
            lon.text(coords[1]);

            var address = item.find("#address_" + itemId).text();
            var name = item.find("#name_" + itemId).text();

            if (data) {
                console.log(data);
                latVal = data.latitude;
                lonVal = data.longitude;

                lat.text(latVal);
                lat.removeClass("badge-danger");
                lat.addClass("badge-light");

                lon.text(lonVal);
                lon.removeClass("badge-danger");
                lon.addClass("badge-light");
            }

            if (stack.children().length >= 5) {
                stack.children()[0].remove();
            }
            var alertid = (new Date().getTime());
            var header = "Сохранение координат";
            var body = "<p>" + (data != null ? "Удалось" : "Не удалось") + " сохранить координаты:<br>";
            body += "Наименование: " + name +
                "<br/>Адрес: " + address +
                "<br/>Широта: " + latVal +
                "<br/>Долгота: " + lonVal + "</p>";
            var message = getAlert("alert_" + alertid, header, body);
            message.addClass(data == null ? "alert-danger" : "alert-success");
            stack.append(message);            
            message.toast('show');
            $('time.timeago').timeago();

        },
        error: function (req, status, error) {
            console.error(error, req, status);
        },
        dataType: "json"
    }); 

    geocode_coord(latVal, lonVal);
}

function setCurrentItem(item) {
    let str = "Old item: " + self.currentItem;
    self.currentItem = $(self.currentItem);
    self.currentItem.removeClass("active");
    self.currentItem = $(item);
    self.currentItem.addClass("active");
    str += " New item: " + self.currentItem;

    let id = self.currentItem.attr("id");
    let lat = self.currentItem.find("#lat_" + id);
    let lon = self.currentItem.find("#lon_" + id);
    let address = self.currentItem.find("#address_" + id);
    let currentItemName = $("#currentItemName");
    currentItemName.text(self.currentItem.find("#name_" + id).text());

    console.debug(str, id, lat.text(), lon.text(), address.text());

    geocode_address(parseFloat(lat.text().replace(",", ".")), parseFloat(lon.text().replace(",", ".")), address.text());
 
    myMap.balloon.close();
}

function geocode_address(lat, lon, address) {
    myMap.geoObjects.removeAll();

    if (lat > 0 && lon > 0) {
        console.debug(lat, lon);
        geocode_coord(lat, lon);
        return;
    }

    ymaps.geocode(address, {
        /**
         * Опции запроса
         * @see https://api.yandex.ru/maps/doc/jsapi/2.1/ref/reference/geocode.xml
         */
        // Сортировка результатов от центра окна карты.
        // boundedBy: myMap.getBounds(),
        // strictBounds: true,
        // Вместе с опцией boundedBy будет искать строго внутри области, указанной в boundedBy.
        // Если нужен только один результат, экономим трафик пользователей.
        results: 1
    }).then(function (res) {
        // Выбираем первый результат геокодирования.
        let firstGeoObject = res.geoObjects.get(0),
            // Координаты геообъекта.
            coords = firstGeoObject.geometry.getCoordinates(),
            // Область видимости геообъекта.
            bounds = firstGeoObject.properties.get('boundedBy');

        firstGeoObject.options.set('preset', 'islands#darkBlueDotIconWithCaption');
        // Получаем строку с адресом и выводим в иконке геообъекта.
        firstGeoObject.properties.set('iconCaption', firstGeoObject.getAddressLine());

        // Добавляем первый найденный геообъект на карту.
        myMap.geoObjects.add(firstGeoObject);
        // Масштабируем карту на область видимости геообъекта.
        myMap.setBounds(bounds, {
            // Проверяем наличие тайлов на данном масштабе.
            checkZoomRange: true
        });

        console.debug('Координаты: ', coords);

        /**
         * Все данные в виде javascript-объекта.
         */
        console.debug('Все данные геообъекта: ', firstGeoObject.properties.getAll());
        /**
         * Метаданные запроса и ответа геокодера.
         * @see https://api.yandex.ru/maps/doc/geocoder/desc/reference/GeocoderResponseMetaData.xml
         */
        console.debug('Метаданные ответа геокодера: ', res.metaData);
        /**
         * Метаданные геокодера, возвращаемые для найденного объекта.
         * @see https://api.yandex.ru/maps/doc/geocoder/desc/reference/GeocoderMetaData.xml
         */
        console.debug('Метаданные геокодера: ', firstGeoObject.properties.get('metaDataProperty.GeocoderMetaData'));
        /**
         * Точность ответа (precision) возвращается только для домов.
         * @see https://api.yandex.ru/maps/doc/geocoder/desc/reference/precision.xml
         */
        console.debug('precision', firstGeoObject.properties.get('metaDataProperty.GeocoderMetaData.precision'));
        /**
         * Тип найденного объекта (kind).
         * @see https://api.yandex.ru/maps/doc/geocoder/desc/reference/kind.xml
         */
        console.debug('Тип геообъекта: %s', firstGeoObject.properties.get('metaDataProperty.GeocoderMetaData.kind'));
        console.debug('Название объекта: %s', firstGeoObject.properties.get('name'));
        console.debug('Описание объекта: %s', firstGeoObject.properties.get('description'));
        console.debug('Полное описание объекта: %s', firstGeoObject.properties.get('text'));
        /**
        * Прямые методы для работы с результатами геокодирования.
        * @see https://tech.yandex.ru/maps/doc/jsapi/2.1/ref/reference/GeocodeResult-docpage/#getAddressLine
        */
        console.debug('\nГосударство: %s', firstGeoObject.getCountry());
        console.debug('Населенный пункт: %s', firstGeoObject.getLocalities().join(', '));
        console.debug('Адрес объекта: %s', firstGeoObject.getAddressLine());
        console.debug('Наименование здания: %s', firstGeoObject.getPremise() || '-');
        console.debug('Номер здания: %s', firstGeoObject.getPremiseNumber() || '-');
    });

}

function geocode_coord(lat, lon) {
    myMap.geoObjects.removeAll();

    ymaps.geocode([lat, lon], {
        /**
         * Опции запроса
         * @see https://api.yandex.ru/maps/doc/jsapi/2.1/ref/reference/geocode.xml
         */
        results: 1
    }).then(function (res) {
        let firstGeoObject = res.geoObjects.get(0);
        // Задаем изображение для иконок меток.
        firstGeoObject.options.set('preset', 'islands#darkBlueDotIconWithCaption');
        firstGeoObject.events
            // При наведении на метку показываем хинт с названием станции метро.
            .add('mouseenter', function (event) {
                var geoObject = event.get('target');
                myMap.hint.open(geoObject.geometry.getCoordinates(), geoObject.getPremise());
            })
            // Скрываем хинт при выходе курсора за пределы метки.
            .add('mouseleave', function (event) {
                myMap.hint.close(true);
            });
        // Добавляем коллекцию найденных геообъектов на карту.
        myMap.geoObjects.add(firstGeoObject);
        // Масштабируем карту на область видимости коллекции.
        myMap.setBounds(firstGeoObject.properties.get('boundedBy'), {
            checkZoomRange: true
        });
    });
}

function getAlert(id, header, body) {
    var html = '<div id="' + id + '" class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="10000"> \
        <div class="toast-header"> \
            <strong id="header" class="mr-auto">' + header + '</strong>&nbsp; \
            <small class="text-muted"><time class="timeago" datetime="' + getDate().toISOString() + '"></time></small> \
            <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close"> \
                <span aria-hidden="true">&times;</span> \
            </button> \
        </div> \
        <div id="body" class="toast-body">' + body + '</div> \
    </div>';

    return $(html);
}