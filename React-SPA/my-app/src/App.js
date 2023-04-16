import { useState, useEffect } from "react";
import logo1 from "./RaspberryTwin_off.png";
import logo2 from "./RaspberryTwin_on.png";
import { DigitalTwinsClient } from "@azure/digital-twins-core";
import { InteractiveBrowserCredential } from "@azure/identity";
import "./App.css";

const RASPBERRY_TWIN_ID = "RaspberryTwin";

const credential = new InteractiveBrowserCredential({ clientId: "0a17a7f6-b2d1-4b29-89cf-ff6bc5a41ed4" });
const digitalTwinsClient = new DigitalTwinsClient("https://DT-demo-resource.api.weu.digitaltwins.azure.net", credential);

async function fetchProperties() {
  const twin = await digitalTwinsClient.getDigitalTwin(RASPBERRY_TWIN_ID);
  const temperature = twin.properties.reported.temperature.value;
  const humidity = twin.properties.reported.humidity.value;
  return { temperature, humidity };
}

function App() {
  const [image, setImage] = useState(logo1);
  const [temperature, setTemperature] = useState(null);
  const [humidity, setHumidity] = useState(null);

  useEffect(() => {
    const interval = setInterval(async () => {
      const { temperature, humidity } = await fetchProperties();
      setTemperature(temperature);
      setHumidity(humidity);
      setImage((prevImage) => (prevImage === logo1 ? logo2 : logo1));
    }, 5000);

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <h1>Raspberry Pi Digital Twin</h1>
        <img src={image} className="App-logo" alt="logo" />
        {/* <p>Temperature: {temperature}</p>
        <p>Humidity: {humidity}</p> */}
      </header>
    </div>
  );
}

export default App;
