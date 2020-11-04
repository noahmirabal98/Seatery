import React from "react"
import * as menuService from "../../services/menus/menuService"
import logger from "sabio-debug"
import PropTypes from "prop-types"
import MenuCard from "./MenuCard";
import ItemTypeCard from "./ItemTypeCard"
import styles from "../menus/menu.module.css"
import vendorServices from "../../services/vendorServices";
import * as productService from "../../services/productService"
import {Link} from "react-router-dom"
import FullMenu from "./FullMenu"
import { toast } from "react-toastify";
import "../../styles/body.css"


const _logger = logger.extend("menu");


class Menu extends React.Component {
  state = {
      pageIndex: 1,
      pageSize: 12,
      totalCount: 0,
      vendorId: this.props.location.state,
      productType: 0,
      show: false,
  };

  componentDidMount = () => {
    this.getProductTypes(this.state.vendorId)
  }

  showAllMenuItems = () => {
    this.getAllVendorItemsById(this.state.pageIndex)
  }

  getByVendorId = (vendorId) => {
    vendorServices
    .getVendorById(vendorId)
    .then(this.onGetVendorByIdSuccess)
    .catch(this.onGetVendorByIdError)
  }

  onGetVendorByIdSuccess = (response) => {
    let vendorInfo = response.data.item;
    this.setState(() => {
      return vendorInfo;
    },() =>this.getAllVendorItemsById(this.state.pageIndex));

  }

  onGetVendorByIdError = (errResponse) => {
    _logger(errResponse);
    toast.warning("Select another restaurant");
  }

  getAllVendorItemsById = (pageIndex) => {
    menuService
    .getByVendorId(pageIndex - 1, this.state.pageSize, this.state.vendorId)
    .then(this.onGetByIdSuccess)
    .catch(this.onGetByIdError);
  }

  onGetByIdSuccess = (response) => {
    let vendorItems = response.item.pagedItems
    let totalCount = response.item.totalCount
    let mappedMenu = vendorItems.map(this.mapItems);
    let  productTypeOptions = this.state.prodTypeData.map(this.mapTypeOptions)

    this.setState((prevState) => {
      
      return {
        ...prevState,
        vendorItems,
        totalCount,
        mappedMenu,
        productTypeOptions
      };
    });
  };

  onGetByIdError = (errResponse) => {
    _logger(errResponse)
    toast.warning("Sorry, no items yet..");
  }

  getProductTypes = (vendorId) => {
    productService
      .getTypeByVendorId(vendorId)
      .then(this.onGetTypeSuccess)
      .catch(this.onGetTypeFailure)
  };

  onGetTypeSuccess = (response) => {
    _logger(response)
    const prodTypeData = response.items;
    this.setState(() => {
      return {
        prodTypeData
      };
    },() => this.getByVendorId(this.state.vendorId));
  };

  onGetTypeFailure = (errResponse) => {
    _logger(errResponse);
    toast.warning("Please select a different restaurant");
  }

  getProductsByType = () => {
    productService
    .getProductByVendorAndTypeId(this.state.pageIndex - 1, this.state.pageSize, this.state.vendorId, this.state.productType)
    .then(this.onGetByIdSuccess)
    .catch(this.onGetProductsByTypeError);
  }

  onGetProductsByTypeError = (errResponse) => { 
    _logger(errResponse)
    toast.warning("Select another type")
  }

  mapTypeOptions = (productTypes, index) => (
      <ItemTypeCard
        key={`${productTypes.id}_${index}`} 
        value={productTypes.id}
        aProductType={productTypes}
        filterTypes={this.filterTypes}
      />
    );

  mapItems = (vendorItems) => (
    <MenuCard
      aVendorItem={vendorItems}
      key={vendorItems.id}
      updateCart={this.props.updateCart}
    />
  );

  filterTypes = (aProductType) => {
    let currentTarget = aProductType
    let newValue = currentTarget.id
    _logger(newValue)
    this.setState(() => {
      return { productType: Number(newValue) };
    },
    () =>
        this.state.productType > 0
          ? this.getProductsByType(1)
          : this.getAllVendorItemsById(1)
    );
  }

  render() {
      return (
        <React.Fragment>
          <div className={`${styles.background}`}>
            <Link style={{fontSize: "30px"}} to="/vendor/landing" className={`${styles.backButton} previous ${styles.previous} round ${styles.round}`}>&#8678;</Link>
            <img className={styles.backgroundImage} src={"https://img.cdn4dd.com/cdn-cgi/image/fit=contain,width=1920,format=auto,quality=50/https://cdn.doordash.com/media/store%2Fheader%2F5579.png"}></img>
            <div className="p-4 text-center text-black">
              <h3 className="m-0" type="button" onClick={this.showAllMenuItems}>{this.state.name}</h3>
              <p className="mb-auto">{this.state.description}</p>
            </div>
          </div>
          <div className={`${styles.scroll} scrollmenu`}>
            <div className={`${styles.typeCards} row justify-content-center`}><FullMenu showAllMenuItems={this.showAllMenuItems}/>{this.state.productTypeOptions}</div>
          </div>
          <div className="container" style={{position: "relative"}}>
            <div className={`${styles.menuRow} row`}>{this.state.mappedMenu}</div>
          </div>
        </React.Fragment>
      );
  }
};


Menu.propTypes = {
  aVendorItem: PropTypes.shape({
    name: PropTypes.string,
    images:PropTypes.arrayOf(PropTypes.shape({
      url: PropTypes.string
    })),
    cost: PropTypes.number,
  }),
  location: PropTypes.shape({
    state: PropTypes.number,
  }),
  vendorTypes: PropTypes.arrayOf(PropTypes.shape({

  })),
  updateCart: PropTypes.func,
};



export default Menu;
