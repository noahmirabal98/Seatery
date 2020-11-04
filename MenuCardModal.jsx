import React, {forwardRef, useImperativeHandle, useState} from "react"
import Modal from "react-bootstrap/Modal"
import Button from "react-bootstrap/Button"
// import Form from "react-bootstrap/Form"
import PropTypes from "prop-types"
import * as cartService from "../../services/cartService"
import styles from "./menu.module.css"
import FormControl from "react-bootstrap/FormControl"
import { Formik } from 'formik';
import logger from "sabio-debug"
import "./modalSpinner.css"

const _logger = logger.extend("cardModal");


let MenuCardModal = forwardRef((props, ref) => {
    MenuCardModal.displayName = "MenuCardModal"

    const [show, setShow] = useState(false);
    const [quantity, setQuantity] = useState(1);
    const [requests, setRequests] = useState(' ');
    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);

    useImperativeHandle(ref, () => {
        return{
            handleShow: handleShow
        };
    });

   let addToCartClick = () => {
    let cartData = {
        "productId": props.aVendorItem.aVendorItem.id,
        "quantity": quantity,
        "specialRequests": requests.toString()
    };
    handleClose()
    cartService
        .addItem(cartData)
        .then(onCartAddSuccess)
        .catch(onCartAddError)
    }   

    let onCartAddSuccess = (response) =>{
        _logger(response)
        cartService
        .getUserCart()
        .then(getCartQuantity)
    }

    let getCartQuantity = (response) => {
        _logger(response)
        let cartItems = response.data.item
        let count = cartItems.length
        props.updateCart(count)
    }

    let  onCartAddError = (errResponse) => {
        _logger(errResponse)
    }

    let handleNumber = (number)=> {
        _logger(number)
        setQuantity(number)
    };

    let handleSpecRequest = (req) => {
        setRequests(req)
    }

    
    let itemTotal = props.aVendorItem.aVendorItem.cost * quantity;
        
    
        return (
            <>
            <Modal show={show} onHide={handleClose}>
                <Modal.Header closeButton className={styles.modalHeader}>
                    <div className={`${styles.modalDescription}`}>{props.aVendorItem.aVendorItem.description}</div>
                    <Modal.Title className={styles.modalTitle}>{props.aVendorItem.aVendorItem.name}</Modal.Title>
                </Modal.Header>
                <Modal.Body className={styles.modalBody}>
                    <div className={`${styles.modalImageContainer} container`}>
                        <img src={props.aVendorItem.aVendorItem.images[0].url} className={styles.modalImage}/>
                    </div>
                    <h5 className={styles.specialRequests}>Special Requests:</h5>
                    <Formik 
                        enableReinitialize={true}
                        initialValues={requests}
                        onSubmit={addToCartClick}
                    >
                    <FormControl maxLength={150} value={requests} onChange={event => handleSpecRequest(event.target.value)} as="textarea" aria-label="With textarea" />
                    </Formik>
                </Modal.Body>
                <Modal.Footer>
                        <Button variant="secondary" value={quantity} onClick={event => handleNumber(event.target.value--)}>-</Button>
                            <input
                                type="number"
                                className={`${styles.spinner} mx-0`}
                                onChange={event => handleNumber(event.target.value)}
                                min="1"
                                max="100"
                                value={quantity}
                            />
                        <Button variant="secondary" value={quantity} onClick={event => handleNumber(event.target.value++)}>+</Button>  
                    <Button className={styles.addToCartButton} variant="primary" onClick={addToCartClick}>
                        {`Add to Cart: $${itemTotal.toFixed(2)}`}
                    </Button>
                </Modal.Footer>
            </Modal>
            </>
          );
    });

MenuCardModal.propTypes = {
    aVendorItem: PropTypes.shape({
        id: PropTypes.number,
        cost: PropTypes.number,
        aVendorItem: PropTypes.shape({
        id: PropTypes.number,
        name: PropTypes.string,
        description: PropTypes.string,
        cost: PropTypes.number,
        images: PropTypes.arrayOf(PropTypes.shape({
            url: PropTypes.string,
    })),}),}),
    onMenuItemClick: PropTypes.func,
    updateCart: PropTypes.func
};


export default MenuCardModal;