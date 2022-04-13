import { Route, Routes } from "react-router-dom";
import About from "./pages/about";
import Entrar from "./pages/entrar";
import Home from "./pages/home";

export default function Rota() {
    return (
        <Routes>
            <Route path='/' element={<Home />} />
            <Route path='/pingas' element={<About />} />
            <Route path='/entrar' element={<Entrar />} />
        </Routes>
    );
}
