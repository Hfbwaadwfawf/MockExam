document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;
    //Text To Speech
    function speakText() {
        let text = document.body.innerText;
        let speech = new SpeechSynthesisUtterance(text);
        speech.lang = "en-GB";
        window.speechSynthesis.speak(speech);
    }
    window.speakText = speakText;
});