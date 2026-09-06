// easy_localization exports its own TextDirection class (an intl/Bidi text-direction
// helper, not Flutter's enum) which otherwise shadows flutter/material.dart's
// TextDirection.ltr/.rtl used below - hidden here since this file never needs
// easy_localization's version.
import 'package:easy_localization/easy_localization.dart' hide TextDirection;
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/app_router.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _identifierCtrl = TextEditingController();
  final _passwordCtrl = TextEditingController();
  final _tenantCodeCtrl = TextEditingController();
  bool _obscurePassword = true;

  @override
  void dispose() {
    _identifierCtrl.dispose();
    _passwordCtrl.dispose();
    _tenantCodeCtrl.dispose();
    super.dispose();
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    context.read<AuthBloc>().add(
          LoginRequested(
            identifier: _identifierCtrl.text.trim(),
            password: _passwordCtrl.text,
            tenantCode: _tenantCodeCtrl.text.trim(),
          ),
        );
  }

  void _biometricLogin() {
    // TODO: retrieve stored deviceId from secure storage
    context.read<AuthBloc>().add(
          const BiometricRequested(deviceId: 'this-device-id'),
        );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: BlocConsumer<AuthBloc, AuthState>(
        listener: (context, state) {
          if (state is AuthMfaRequired) {
            context.go(AppRoutes.mfa, extra: state.sessionToken);
          } else if (state is AuthAuthenticated) {
            context.go(AppRoutes.home);
          } else if (state is AuthFailure) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.failure.message)),
            );
          }
        },
        builder: (context, state) {
          return SafeArea(
            child: Center(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 24),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      const SizedBox(height: 40),
                      const FlutterLogo(size: 60),
                      const SizedBox(height: 32),
                      Text(
                        'auth.login.title'.tr(),
                        style: Theme.of(context).textTheme.headlineSmall,
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 32),

                      // Tenant code
                      TextFormField(
                        controller: _tenantCodeCtrl,
                        textDirection: TextDirection.ltr,
                        decoration: InputDecoration(
                          labelText: 'Tenant Code',
                          border: const OutlineInputBorder(),
                          prefixIcon: const Icon(Icons.business),
                        ),
                        validator: (v) => v == null || v.isEmpty
                            ? 'Tenant code is required'
                            : null,
                      ),
                      const SizedBox(height: 16),

                      // Identifier
                      TextFormField(
                        controller: _identifierCtrl,
                        textDirection: TextDirection.ltr,
                        keyboardType: TextInputType.emailAddress,
                        decoration: InputDecoration(
                          labelText: 'auth.iqama_or_username'.tr(),
                          border: const OutlineInputBorder(),
                          prefixIcon: const Icon(Icons.person),
                        ),
                        validator: (v) => v == null || v.isEmpty
                            ? 'Identifier is required'
                            : null,
                      ),
                      const SizedBox(height: 16),

                      // Password
                      TextFormField(
                        controller: _passwordCtrl,
                        obscureText: _obscurePassword,
                        decoration: InputDecoration(
                          labelText: 'auth.password'.tr(),
                          border: const OutlineInputBorder(),
                          prefixIcon: const Icon(Icons.lock),
                          suffixIcon: IconButton(
                            icon: Icon(
                              _obscurePassword
                                  ? Icons.visibility_off
                                  : Icons.visibility,
                            ),
                            onPressed: () => setState(
                              () => _obscurePassword = !_obscurePassword,
                            ),
                          ),
                        ),
                        validator: (v) => v == null || v.isEmpty
                            ? 'Password is required'
                            : null,
                      ),
                      const SizedBox(height: 24),

                      // Login button
                      FilledButton(
                        onPressed: state is AuthLoading ? null : _submit,
                        child: state is AuthLoading
                            ? const SizedBox(
                                height: 20,
                                width: 20,
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                  color: Colors.white,
                                ),
                              )
                            : Text('auth.login.title'.tr()),
                      ),
                      const SizedBox(height: 12),

                      // Biometric button
                      OutlinedButton.icon(
                        onPressed: state is AuthLoading ? null : _biometricLogin,
                        icon: const Icon(Icons.fingerprint),
                        label: Text('auth.biometric.prompt'.tr()),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}
