import { useState, useEffect } from 'react';
import logo1 from './RaspberryTwin_on.png';
import logo2 from './RaspberryTwin_off.png';
import './App.css';

const apiEndpoint = `https://DT-demo-resource.api.weu.digitaltwins.azure.net/digitaltwins/RaspberryTwin/properties`;
const accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiJodHRwczovL2RpZ2l0YWx0d2lucy5henVyZS5uZXQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wZmQ1YjU0OC1hM2FlLTQ3NmYtODg2Zi0wZGY0MGMxOTcyOTMvIiwiaWF0IjoxNjgxNjc1OTMxLCJuYmYiOjE2ODE2NzU5MzEsImV4cCI6MTY4MTY4MDk1NiwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQWhlTnBxa1paUVc4dFhoUU9qOXgxNkoyN3puZTZxelRuemRoN3RHanBNVFBZck9FMFY1UkhxTHJ3U0JQMWc4T2J1bzhuNTFHYzBteGV5NWZxRk9oRWZLdHdXZHJESFFhc2tzalVyN2EwbUFJPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiIwNGIwNzc5NS04ZGRiLTQ2MWEtYmJlZS0wMmY5ZTFiZjdiNDYiLCJhcHBpZGFjciI6IjAiLCJmYW1pbHlfbmFtZSI6IldpbnNzZW4gdmFuIiwiZ2l2ZW5fbmFtZSI6IkxlbmFyZCIsImlwYWRkciI6IjIxMy4xMC44My4yMjEiLCJuYW1lIjoiV2luc3NlbiB2YW4sIExlbmFyZCIsIm9pZCI6IjJlN2ZjMzBiLTFkMzAtNDk5OC1iM2Q1LWE3OGM4MWM0Nzc1NyIsIm9ucHJlbV9zaWQiOiJTLTEtNS0yMS00MTQ1MjM3Mjk1LTMyNTExMDY4MjQtOTYxODY5ODg2LTU1MjMxIiwicHVpZCI6IjEwMDNCRkZEQUQ0RDdENUYiLCJyaCI6IjAuQVhNQVNMWFZENjZqYjBlSWJ3MzBEQmx5a3luMEJ3dExueFJIazVMTVhvNkF5TEJ6QUFRLiIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6InZEYnd0c1JqOEFuTkNzVHRRbTUwcllaRGRhTVBiNU0zX20yRzZzOF9URzAiLCJ0aWQiOiIwZmQ1YjU0OC1hM2FlLTQ3NmYtODg2Zi0wZGY0MGMxOTcyOTMiLCJ1bmlxdWVfbmFtZSI6Imx2d2luc3NlbkBzdHVkZW50LmNoZS5ubCIsInVwbiI6Imx2d2luc3NlbkBzdHVkZW50LmNoZS5ubCIsInV0aSI6InJsZ2pOQ0dzS2t5NTB3LWlBdzRvQUEiLCJ2ZXIiOiIxLjAifQ.G1a5qHoZz-_9gIt6PzJd2q3ZjHIvRh_HYgS1MeNokJ124vuVWCYuKuMkaYaHiIxVK_mAdB6Nz1FHS0j3Pc7eBlfWewpLLp40VGrkHvyQ_m9Th3MJO88Q6tbRkIdLeGL_84rTVUP7VWIEBVUSAyzfh3BbUKIxczEVEqhITeWF8thm53JrQDQRsK8h2M_LSTFOHzbmJ8BaNhGaZCOF1ZtH_CEuJrhZcZsuJJOtMkq-2giaLPLu8CgYm_nFApiTZeWSH3CqoscgAlp-91XHsvBLnOPsiBFoK8X4H_gFxpkZ-3xSinwlCZPdeMCFQ84_YezLZpTW8uheGiB98sUuT_Qgng";

fetch(apiEndpoint, {
  method: "GET",
  headers: {
    Authorization: `Bearer ${accessToken}`
  }
})
  .then(response => response.json())
  .then(data => {
    const temperature = data.temperature.value;
    const humidity = data.humidity.value;
  })
  .catch(error => {
    console.error(error);
  });

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
        <p>Temperature: {fetch.temperature}</p>
        <p>Humidity: {fetch.humidity}</p>
      </header>
    </div>
  );
}

export default App;
