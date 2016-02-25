
function getWeekNumber(d) {
    // Copy date so don't modify original
    d = new Date(d);
    d.setHours(0, 0, 0);
    // Set to nearest Thursday: current date + 4 - current day number
    // Make Sunday's day number 7
    d.setDate(d.getDate() + 4 - (d.getDay() || 7));
    // Get first day of year
    var yearStart = new Date(d.getFullYear(), 0, 1);
    // Calculate full weeks to nearest Thursday
    var weekNo = Math.ceil((((d - yearStart) / 86400000) + 1) / 7)

    return pad(weekNo, 2);
}

function getQuarter(d) {
    d = d || new Date();
    var m = Math.floor(d.getMonth() / 3) + 1;
    return m;
}

function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function padRigth(number, length) {
    var str = '' + number;
    while (str.length < length) {
        str = str + '0';
    }

    return str;
}

Global.ClientTraceEventsService = {

    aasService: 'ClientTraceEventsService',
    aasPublished: true,

    GetWeekNumber: function (key) {
        if (key) {
            return key.substr(7, 2);
        }
        return getWeekNumber(new Date());
    },

    GetQuarterNumber: function (key) {
        if (key) {
            return key.substr(4, 1);
        }
        return getQuarter();
    },

    SelectTag: function (dataSetName, tagId, frequency, minDate, maxDate, graphControlId) {
        var em = Aspectize.EntityManagerFromContextDataName(dataSetName);

        var tag = em.GetInstance('Tag', { Id: tagId });

        var enumFrequency = em.GetInstance('EnumFrequency', { EnumerationValue: frequency });

        var minDateString = Aspectize.formatString('{0:yyyy}', minDate);

        var tempDate = new Date(maxDate.valueOf());
        var maxDateTomorrow = new Date(tempDate.setTime(tempDate.getTime() + 86400000));
        var maxDateString = Aspectize.formatString('{0:yyyy}', maxDateTomorrow);

        if (enumFrequency.EnumerationValue <= 32) {
            minDateString = minDateString + getQuarter(minDate);
            maxDateString = maxDateString + getQuarter(maxDateTomorrow);
        }

        if (enumFrequency.EnumerationValue <= 16) {
            minDateString = minDateString + Aspectize.formatString('{0:MM}', minDate);
            maxDateString = maxDateString + Aspectize.formatString('{0:MM}', maxDateTomorrow);
        }

        if (enumFrequency.EnumerationValue <= 8) {
            minDateString = minDateString + getWeekNumber(minDate);
            maxDateString = maxDateString + getWeekNumber(maxDateTomorrow);
        }

        if (enumFrequency.EnumerationValue <= 4) {
            minDateString = minDateString + Aspectize.formatString('{0:dd}', minDate);
            maxDateString = maxDateString + Aspectize.formatString('{0:dd}', maxDateTomorrow);
        }

        if (enumFrequency.EnumerationValue <= 2) {
            minDateString = minDateString + Aspectize.formatString('{0:HH}', minDate);
            maxDateString = maxDateString + Aspectize.formatString('{0:HH}', maxDateTomorrow);
        }

        if (enumFrequency.EnumerationValue <= 1) {
            minDateString = minDateString + Aspectize.formatString('{0:mm}', minDate);
            maxDateString = maxDateString + Aspectize.formatString('{0:mm}', maxDateTomorrow);
        }

        // Patch pour les semaines à cheval sur 2 mois ou quarter
        if (enumFrequency.EnumerationValue == 8) {
            minDateString = Aspectize.formatString('{0:yyyy}', minDate) + '000' + getWeekNumber(minDate);
            maxDateString = Aspectize.formatString('{0:yyyy}', maxDateTomorrow) + '000' + getWeekNumber(maxDateTomorrow);
        }

        minDateString = padRigth(minDateString, 17);
        maxDateString = padRigth(maxDateString, 17);

        var serie = enumFrequency.EnumerationDescription;

        var graphControlId = graphControlId + '-Chart' + serie;

        var series = tag.GetAssociated('Tag' + serie, serie);

        var column = 'Count';

        if (tag.ValueType == 1) {
            column = 'Sum';
        }

        var minValue = 0; var maxValue = 0;

        for (var i = 0; i < series.length; i++) {
            var serieData = series[i];

            var serieId = serieData.Id;

            var parts = serieId.split('|');

            if (parts.length > 1) {
                var dateSerie = parts[2];

                var dateSeriePad = padRigth(dateSerie, 12);

                if (minDateString <= dateSeriePad && maxDateString >= dateSeriePad) {
                    var valueSerie = serieData[column + serie];
                    if (!minValue || (valueSerie <= minValue)) {
                        minValue = valueSerie;
                    }

                    if (!maxValue || (maxValue <= valueSerie)) {
                        maxValue = valueSerie;
                    }

                    serieData.SetField('Display', true);
                }
                else {
                    serieData.SetField('Display', false);
                }
            }
        }

        tag.SetField('MinValue', minValue);
        tag.SetField('MaxValue', maxValue);

        var dhtmlxChartService = Aspectize.Host.GetService('DhtmlxChartService');

        tag.SetField('GraphMin', dhtmlxChartService.GetGraphBegin(minValue, maxValue));
        tag.SetField('GraphMax', dhtmlxChartService.GetGraphEnd(minValue, maxValue));
        tag.SetField('GraphStep', dhtmlxChartService.GetGraphStep(minValue, maxValue));

        dhtmlxChartService.RefreshGraph(graphControlId);
    },

    GetGraphSerieElement: function (dataSetName, tagId, serie, minDate, maxDate, typeElement) {
        var em = Aspectize.EntityManagerFromContextDataName(dataSetName);

        var tag = em.GetInstance('Tag', { Id: tagId });

        var series = tag.GetAssociated('Tag' + serie, serie);

        var minValue; var maxValue;

        for (var i = 0; i < series.length; i++) {
            var serieData = series[i];

            var serieId = serieData.Id;

            var parts = serieId.split('|');

            if (parts.length > 1) {
                var dateSerie = parts[2];

                var minDateString = Aspectize.formatString('{0:yyyyMMddHHmm}', minDate);
                var maxDateString = Aspectize.formatString('{0:yyyyMMddHHmm}', maxDate);

                var dateSeriePad = padRigth(dateSerie, 12);

                if (minDateString <= dateSeriePad && maxDateString >= dateSeriePad) {
                    var valueSerie = serieData['Count' + serie];
                    if (!minValue || (valueSerie <= minValue)) {
                        minValue = valueSerie;
                    }

                    if (!maxValue || (valueSerie <= maxValue)) {
                        maxValue = valueSerie;
                    }
                }
            }
        }

        var result = this['GetGraph' + typeElement](minValue, maxValue);

        return result;
    }

};

