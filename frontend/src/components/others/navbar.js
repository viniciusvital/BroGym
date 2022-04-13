import '../../css/navbar.css';

export default function Navbar() {
    return (
        <nav className="nav">
            <span className="nav-logo">BROGYM</span>
            <ul>
                <li>Sobre</li>
                <div className="nav-dropdown">
                    <a href="#">Instrutores</a>
                    <a href="#">Aulas</a>
                    <a href="#">Preços</a>
                </div>
                <li>Agenda</li>
                <li>Contato</li>
                <li>Loja</li>
            </ul>
            <span>Entrar</span>
            <div className="nav-socials">
                <a href="#">IG</a>
                <a href="#">YT</a>
                <a href="#">FB</a>
            </div>
            <div className="Nav-buycart">CART</div>
        </nav>
    );
}
