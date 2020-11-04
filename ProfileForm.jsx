import React from "react";
import logger from "sabio-debug";
import * as profileService from "../../services/profiles/profileService";   
import { Form, FormGroup, Label, Button } from "reactstrap";
import { Formik, Field, ErrorMessage } from "formik";
import PropTypes from "prop-types";
import styles from "../profiles/profile.module.css"
import Validation from "./Validation"
import Upload from "../files/Upload"


const _logger = logger.extend("SabioInit");

class ProfileForm extends React.Component {
  state = {
    profileData: {
      firstName: "",
      lastName: "",
      mi: "",
      avatarUrl: "",
    },
  };

  componentDidMount = () => {
    if (this.props.match.params.id) {
     if (this.props.location.state) {
       this.setFormData(this.props.location.state.payload)
     }
    }
  };

setFormData = (profileData) =>{
  this.setState(() => {
    return {
      profileData
    };
  });
}

  handleSubmit = (values, { resetForm }) => {
    _logger(values);
    const data = {
      firstName: values.firstName,
      lastName: values.lastName,
      mi: values.mi,
      avatarUrl: values.avatarUrl
    }
    if(this.props.match.params.id) {
      profileService
      .update(data, this.props.match.params.id)
      .then(this.onUpdateSuccess)
      .then(resetForm(this.state.profileData))
      .catch(this.onUpdateError)
    } 
    else {
      profileService
      .add(data)
      .then(this.onAddSuccess)
      .catch(this.onAddError);
    resetForm(this.state.profileData); 
    }
    
  };

  onAddSuccess = (response) => {
    _logger(response);
    this.props.history.push("/profile")
  };
  onAddError = (response) => {
    _logger(response);
  };

  onUpdateSuccess = (response) => {
    _logger(response)
    this.props.history.push("/profile")
  }
  onUpdateError = (response) => {
    _logger(response)
  }

  render() {
    return (
      <React.Fragment>
        <Formik
          enableReinitialize={true}
          initialValues={this.state.profileData}
          onSubmit={this.handleSubmit}
          validationSchema={Validation}
        >
          {(props) => {
            const { values, handleSubmit, isValid, isSubmitting } = props;
            return (
              <div className={`${styles.addForm} card-default card`}>
                <div className={`card-header ${styles.cardTitle}`}>My Profile</div>
                <div className={`${styles.cardBody} card-body`}>
                  <Form onSubmit={handleSubmit}>
                    <FormGroup>
                      <Label>First Name</Label>
                      <Field
                        placeholder="first name"
                        type="text"
                        className="form-control"
                        name="firstName"
                        values={values.firstName}
                      />
                      <ErrorMessage
                        component="span"
                        name="firstName"
                        className={`${styles.errorMessage}`}
                      />
                    </FormGroup>
                    <FormGroup>
                      <Label>Middle Initial</Label>
                      <Field
                        placeholder="mi"
                        type="text"
                        className="form-control"
                        name="mi"
                        values={values.mi}
                      />
                      <ErrorMessage
                        component="span"
                        name="mi"
                        className={`${styles.errorMessage}`}
                      />
                    </FormGroup>
                    <FormGroup>
                      <Label>Last Name</Label>
                      <Field
                        placeholder="last name"
                        type="text"
                        className="form-control"
                        name="lastName"
                        values={values.lastName}
                      />
                      <ErrorMessage
                        component="span"
                        name="lastName"
                        className={`${styles.errorMessage}`}
                      />
                    </FormGroup>
                    <FormGroup>
                      <Label>Profile Photo</Label>
                      <Upload></Upload>
                      <Field
                        placeholder="url"
                        type="text"
                        className="form-control"
                        name="avatarUrl"
                        values={values.avatarUrl}
                      />
                      <ErrorMessage
                        component="span"
                        name="avatarUrl"
                        className={`${styles.errorMessage}`}
                      />
                    </FormGroup>
                    <Button
                      className={`${styles.doneButton} btn btn-primary`}
                      type="submit"
                      onSubmit={this.handleSubmit}
                      disabled={!isValid || isSubmitting}
                    >
                      {this.props.match.params.id ? "Update" : "Done"}
                    </Button>
                  </Form>
                </div>
              </div>
            );
          }}
        </Formik>
      </React.Fragment>
    );
  }
}

ProfileForm.propTypes = {
  match: PropTypes.shape({
    params: PropTypes.shape({
      id: PropTypes.string,
    }),
  }),
  location: PropTypes.shape({
    state: PropTypes.shape({
      payload: PropTypes.shape({
        firstName: PropTypes.string,
        lastName: PropTypes.string,
        mi: PropTypes.string,
        avatarUrl: PropTypes.string,
        userId: PropTypes.number,
      })
    })
  }),
  history: PropTypes.shape({
    push: PropTypes.func
  })
};

export default ProfileForm;
