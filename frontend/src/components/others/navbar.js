import { faFacebookF, faInstagram, faYoutube } from '@fortawesome/free-brands-svg-icons';
import { faCartShopping } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import '../../css/navbar.css';

export default function Navbar() {
    return (
        <div className="navbar">
            <div className="nav-logo">BROGYM</div>
            <div className="nav-menu">
                <ul>
                    <li>Sobre</li>
                    <li>Agenda</li>
                    <li>Contato</li>
                    <li>Loja</li>
                </ul>
            </div>
            <div>
            <a href={()=>false}>Entrar</a>
            </div>
            <div className="nav-socials">
                <a href={()=>false}><FontAwesomeIcon icon={faInstagram} size='lg'/></a>
                <a href={()=>false}><FontAwesomeIcon icon={faYoutube} size='lg' /></a>
                <a href={()=>false}><FontAwesomeIcon icon={faFacebookF} size='lg' /></a>
            </div>

            <div className="nav-buycart">
            <a href={()=>false}><FontAwesomeIcon icon={faCartShopping} size='lg' /></a>
            </div>
        </div>
    );
}
