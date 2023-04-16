import { useState, useEffect } from 'react';
import logo1 from './RaspberryTwin_off.png';
import logo2 from './RaspberryTwin_on.png';
import './App.css';

function App() {
  const [image, setImage] = useState(logo1);

  useEffect(() => {
    const interval = setInterval(() => {
      setImage((prevImage) => prevImage === logo1 ? logo2 : logo1);
    }, 500);

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <h1>Raspberry Pi Digital Twin</h1>
        <img src={image} className="App-logo" alt="logo" />
        <p>Temperature:</p>
        <p>Humidity:</p>
      </header>
    </div>
  );
}

export default App;