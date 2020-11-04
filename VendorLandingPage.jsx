import React from "react"
import vendorServices from "../../services/vendorServices"
import logger from "sabio-debug"
import VendorCard from "./VendorCard"
// import styles from "../menus/menu.module.css"
import stylez from "../profiles/profile.module.css";
import "../../styles/body.css"
import { toast } from "react-toastify";


const _logger = logger.extend("vendorLandingPage");


class VendorLandingPage extends React.Component {
    state = {
        pageIndex: 1,
        pageSize: 30,
        searchBar: "",
    }


    componentDidMount = () => {
        this.onGetVendors(this.state.pageIndex)
    }

    onGetVendors = (pageIndex) => {
      vendorServices
      .getVendors(pageIndex - 1, this.state.pageSize)
      .then(this.onGetVendorsSuccess)
      .catch(this.onGetVendorsError)
    }

    onGetVendorsSuccess  = (response) => {
      let vendors = response.data.item.pagedItems;
      _logger(vendors);
      let totalCount = response.data.item.totalCount;
      this.setState(() => {
        return {
          mappedVendors: vendors.map(this.mapVendors),
          totalCount,
        };
      });
    };
  
    onGetVendorsError = (errResponse) => {
      _logger(errResponse);
      toast.error("Check your connection")
    };

    mapVendors = (vendors) => {
      return (
        <VendorCard
          aVendor={vendors}
          key={vendors.id}
        ></VendorCard>
      );
    };

    onFormFieldChanged = (e) => {
      let currentTarget = e.currentTarget;
      let newValue = currentTarget.value;
  
      this.setState(
        () => {
          return { searchBar: newValue };
        },
        () =>
          this.state.searchBar.length > 0
            ? this.searchResults(1)
            : this.onGetVendors(1)
      );
    };

    searchResults = (page) => {
      let name = this.state.searchBar;
      vendorServices
        .getVendorByName(name, page - 1, this.state.pageSize)
        .then(this.onGetVendorsSuccess)
        .catch(this.onErrorByName);
    };

    render() {
        return (
          <React.Fragment>
            <label className={`${stylez.search} search`}>
              <div className="input-group-prepend">
                <span
                  className={`input-group-text ${stylez.searchIconContainer}`}
                >
                  {
                    <em
                      className={`fa-2x mr-2 fas fa-search ${stylez.searchIcon}`}
                    ></em>
                  }
                </span>
              </div>
              <input
                type="text"
                className={`form-control form-control-sm my-0 ${stylez.searchBar}`}
                placeholder="Search"
                aria-controls="DataTables_Table_5"
                value={this.state.searchBar}
                onChange={this.onFormFieldChanged}
                name="searchBar"
              ></input>
            </label>
            <div className="row justify-content-center my-5">
              <h1 >Food Guide</h1>
            </div>
            <div className="container">
              <div className="row">{this.state.mappedVendors}</div>
            </div>
          </React.Fragment>
        );
    }
}


export default VendorLandingPage;