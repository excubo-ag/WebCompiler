window.takeScreenshot = function (dotnet, selector) {
    html2canvas(document.querySelector(selector), { allowTaint: true, scale: 0.1, useCORS: true }).then(
        async (canvas) => {
            var image_data = canvas.toDataURL("image/jpeg", 0.7).toString();
            for (i = 0; i < image_data.length; i = i + 1000) {
                await dotnet.invokeMethodAsync('AppendData', image_data.substring(i, Math.min(image_data.length, i + 1000)))
            }
            await dotnet.invokeMethodAsync('SubmitData')
        }
    )
};