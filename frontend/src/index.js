import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import Footer from './components/others/footer';
import Navbar from './components/others/navbar';
import './css/index.css';
import Rota from './rota';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(

    <BrowserRouter>
        <Navbar />
        <Rota />
        <Footer />
    </BrowserRouter>


);