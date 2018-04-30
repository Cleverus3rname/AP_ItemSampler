import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link, NavLink } from 'react-router-dom';

export interface HomeProps extends RouteComponentProps<{}> {
    appName: string;
}

export class Home extends React.Component<HomeProps, {}> {
    constructor(props: HomeProps) {
        super(props);
    }

    componentDidMount() {
        document.title = `Home - Smarter Balanced ${this.props.appName}`;
    }
    renderWelcomeText() {
        return (
            <p> This site provides examples of test questions used on Smarter Balanced assessments in English language
        arts / literacy and math. Teachers, parents, students, administrators, and policymakers can experience these test items just as students encounter them.</p>
        );
    }

    renderAboutText() {
        return (
            <p>These samples are not intended to be used as practice tests, but educators can use them to better
        understand how Smarter Balanced measures college- and career-ready content.</p>
        );
    }

    renderHomeLinks() {
        return (
            <div className="home-links">
                <NavLink to={'/BrowseItems'} exact activeClassName='active' className="btn-lg btn-primary home-container-button">
                    <span>Browse Test Items</span>
                </NavLink>
                <NavLink to={'/AboutItems'} exact activeClassName='active' className="btn-lg btn-success home-container-button">
                    <span>Learn More About Items</span>
                </NavLink>

            </div >
        );
    }
    renderMain() {
        return (
            <div className="section home-welcome">
                <h1 className="home-title"><b>Welcome!</b></h1>
                <div className="home-content">
                    {this.renderWelcomeText()}
                    {this.renderAboutText()}
                    {this.renderHomeLinks()}
                </div>
            </div >
        );
    }

    public render() {
        return (
            <div className="home-page">

                <div className="home-container">
                    <div className="container">
                        {this.renderMain()}
                    </div>
                </div>

            </div>
        );
    }
}
